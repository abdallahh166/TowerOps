using FluentValidation;
using MediatR;
using TowerOps.Application.Common;
using TowerOps.Domain.Enums;
using TowerOps.Domain.Interfaces.Repositories;

namespace TowerOps.Application.Queries.Privacy.GetMyOperationalDataExport;

public sealed record GetMyOperationalDataExportQuery : IQuery<string>
{
    public Guid UserId { get; init; }
    public Guid RequestId { get; init; }
}

public sealed class GetMyOperationalDataExportQueryValidator : AbstractValidator<GetMyOperationalDataExportQuery>
{
    public GetMyOperationalDataExportQueryValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.RequestId).NotEmpty();
    }
}

public sealed class GetMyOperationalDataExportQueryHandler : IRequestHandler<GetMyOperationalDataExportQuery, Result<string>>
{
    private readonly IUserDataExportRequestRepository _userDataExportRequestRepository;
    private readonly IUnitOfWork _unitOfWork;

    public GetMyOperationalDataExportQueryHandler(
        IUserDataExportRequestRepository userDataExportRequestRepository,
        IUnitOfWork unitOfWork)
    {
        _userDataExportRequestRepository = userDataExportRequestRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<string>> Handle(GetMyOperationalDataExportQuery request, CancellationToken cancellationToken)
    {
        var exportRequest = await _userDataExportRequestRepository.GetByIdForUserAsync(
            request.RequestId,
            request.UserId,
            cancellationToken);

        if (exportRequest is null)
            return Result.Failure<string>("Export request not found.");

        var now = DateTime.UtcNow;
        if (now > exportRequest.ExpiresAtUtc)
        {
            exportRequest.MarkExpired(now);
            await _userDataExportRequestRepository.UpdateAsync(exportRequest, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Failure<string>("Export request has expired.");
        }

        if (exportRequest.Status != UserDataExportStatus.Completed || string.IsNullOrWhiteSpace(exportRequest.PayloadJson))
            return Result.Failure<string>("Export file is not ready.");

        return Result.Success(exportRequest.PayloadJson);
    }
}
