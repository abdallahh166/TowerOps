using System.Text.Json;
using FluentValidation;
using MediatR;
using TowerOps.Application.Common;
using TowerOps.Application.Common.Interfaces;
using TowerOps.Application.DTOs.Privacy;
using TowerOps.Domain.Entities.UserDataExports;
using TowerOps.Domain.Interfaces.Repositories;

namespace TowerOps.Application.Commands.Privacy.RequestMyOperationalDataExport;

public sealed record RequestMyOperationalDataExportCommand : ICommand<UserDataExportRequestDto>
{
    public Guid UserId { get; init; }
}

public sealed class RequestMyOperationalDataExportCommandValidator : AbstractValidator<RequestMyOperationalDataExportCommand>
{
    public RequestMyOperationalDataExportCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
    }
}

public sealed class RequestMyOperationalDataExportCommandHandler : IRequestHandler<RequestMyOperationalDataExportCommand, Result<UserDataExportRequestDto>>
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true
    };

    private readonly IUserRepository _userRepository;
    private readonly IVisitRepository _visitRepository;
    private readonly IWorkOrderRepository _workOrderRepository;
    private readonly IUserDataExportRequestRepository _userDataExportRequestRepository;
    private readonly ISystemSettingsService _settingsService;
    private readonly IUnitOfWork _unitOfWork;

    public RequestMyOperationalDataExportCommandHandler(
        IUserRepository userRepository,
        IVisitRepository visitRepository,
        IWorkOrderRepository workOrderRepository,
        IUserDataExportRequestRepository userDataExportRequestRepository,
        ISystemSettingsService settingsService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _visitRepository = visitRepository;
        _workOrderRepository = workOrderRepository;
        _userDataExportRequestRepository = userDataExportRequestRepository;
        _settingsService = settingsService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<UserDataExportRequestDto>> Handle(RequestMyOperationalDataExportCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsNoTrackingAsync(request.UserId, cancellationToken);
        if (user is null)
            return Result.Failure<UserDataExportRequestDto>("User not found.");

        var maxItems = await _settingsService.GetAsync(
            "Privacy:Export:MaxItemsPerCollection",
            2000,
            cancellationToken);
        var safeMaxItems = Math.Clamp(maxItems, 100, 10000);

        var visits = await BuildVisitSnapshotAsync(request.UserId, safeMaxItems, cancellationToken);
        var workOrders = await _workOrderRepository.GetByUserOwnershipAsNoTrackingAsync(request.UserId, safeMaxItems, cancellationToken);

        var payload = new
        {
            generatedAtUtc = DateTime.UtcNow,
            user = new
            {
                id = user.Id,
                name = user.Name,
                email = user.Email,
                role = user.Role.ToString(),
                officeId = user.OfficeId,
                createdAtUtc = user.CreatedAt,
                lastLoginAtUtc = user.LastLoginAt
            },
            visits = visits.Select(v => new
            {
                v.Id,
                v.VisitNumber,
                v.SiteId,
                v.SiteCode,
                v.SiteName,
                v.EngineerId,
                v.Status,
                v.Type,
                scheduledDateUtc = v.ScheduledDate,
                actualStartUtc = v.ActualStartTime,
                actualEndUtc = v.ActualEndTime,
                completionPercent = v.CompletionPercentage,
                checkInTimeUtc = v.CheckInTimeUtc,
                checkOutTimeUtc = v.CheckOutTimeUtc,
                photos = v.Photos
                    .Where(p => !p.IsDeleted)
                    .Select(p => new
                    {
                        p.Id,
                        p.Type,
                        p.Category,
                        p.ItemName,
                        p.FileName,
                        p.FilePath,
                        p.FileStatus,
                        p.CapturedAtUtc,
                        p.Description,
                        p.ScanCompletedAtUtc,
                        p.QuarantineReason
                    })
                    .ToList(),
                readings = v.Readings
                    .Where(r => !r.IsDeleted)
                    .Select(r => new
                    {
                        r.Id,
                        r.ReadingType,
                        r.Value,
                        r.Unit,
                        r.IsWithinRange
                    })
                    .ToList(),
                checklist = v.Checklists
                    .Where(c => !c.IsDeleted)
                    .Select(c => new
                    {
                        c.Id,
                        c.Category,
                        c.ItemName,
                        c.Status,
                        c.Notes
                    })
                    .ToList(),
                issues = v.IssuesFound
                    .Where(i => !i.IsDeleted)
                    .Select(i => new
                    {
                        i.Id,
                        i.Category,
                        i.Severity,
                        i.Description,
                        i.Status,
                        i.TargetDateUtc
                    })
                    .ToList()
            }).ToList(),
            workOrders = workOrders.Select(w => new
            {
                w.Id,
                w.WoNumber,
                w.SiteCode,
                w.OfficeCode,
                w.WorkOrderType,
                w.Scope,
                w.SlaClass,
                w.Status,
                w.IssueDescription,
                w.AssignedEngineerId,
                w.AssignedEngineerName,
                w.SlaStartAtUtc,
                w.ScheduledVisitDateUtc,
                w.ResponseDeadlineUtc,
                w.ResolutionDeadlineUtc,
                w.CreatedAt,
                w.UpdatedAt
            }).ToList()
        };

        var payloadJson = JsonSerializer.Serialize(payload, SerializerOptions);
        var expiryDays = await _settingsService.GetAsync("Privacy:Export:TtlDays", 30, cancellationToken);
        var exportRequest = UserDataExportRequest.Create(request.UserId, Math.Clamp(expiryDays, 1, 365));
        exportRequest.Complete(payloadJson);

        await _userDataExportRequestRepository.AddAsync(exportRequest, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new UserDataExportRequestDto
        {
            RequestId = exportRequest.Id,
            RequestedAtUtc = exportRequest.RequestedAtUtc,
            ExpiresAtUtc = exportRequest.ExpiresAtUtc,
            Status = exportRequest.Status.ToString()
        });
    }

    private async Task<IReadOnlyList<TowerOps.Domain.Entities.Visits.Visit>> BuildVisitSnapshotAsync(
        Guid userId,
        int maxItems,
        CancellationToken cancellationToken)
    {
        var baseVisits = await _visitRepository.GetByEngineerIdAsNoTrackingAsync(userId, cancellationToken);
        var selected = baseVisits
            .Take(maxItems)
            .ToList();

        var hydrated = new List<TowerOps.Domain.Entities.Visits.Visit>(selected.Count);
        foreach (var visit in selected)
        {
            var full = await _visitRepository.GetByIdAsNoTrackingAsync(visit.Id, cancellationToken);
            if (full is not null)
            {
                hydrated.Add(full);
            }
        }

        return hydrated;
    }
}
