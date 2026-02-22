using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using TelecomPM.Application.Common;
using TelecomPM.Application.Common.Interfaces;
using TelecomPM.Application.Commands.AuditLogs.LogAuditEntry;
using TelecomPM.Application.DTOs.WorkOrders;
using TelecomPM.Domain.Entities.WorkOrders;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Interfaces.Repositories;

namespace TelecomPM.Application.Commands.WorkOrders.CreateWorkOrder;

public class CreateWorkOrderCommandHandler : IRequestHandler<CreateWorkOrderCommand, Result<WorkOrderDto>>
{
    private readonly IWorkOrderRepository _workOrderRepository;
    private readonly ISiteRepository _siteRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ISender _sender;
    private readonly ISystemSettingsService _systemSettingsService;

    public CreateWorkOrderCommandHandler(
        IWorkOrderRepository workOrderRepository,
        ISiteRepository siteRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ISender sender,
        ISystemSettingsService systemSettingsService)
    {
        _workOrderRepository = workOrderRepository;
        _siteRepository = siteRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _sender = sender;
        _systemSettingsService = systemSettingsService;
    }

    public async Task<Result<WorkOrderDto>> Handle(CreateWorkOrderCommand request, CancellationToken cancellationToken)
    {
        var existing = await _workOrderRepository.GetByWoNumberAsync(request.WoNumber, cancellationToken);
        if (existing != null)
            return Result.Failure<WorkOrderDto>($"Work order with number {request.WoNumber} already exists");

        var site = await _siteRepository.GetBySiteCodeAsNoTrackingAsync(request.SiteCode, cancellationToken);
        if (site is null)
            return Result.Failure<WorkOrderDto>("Site not found.");

        if (site.ResponsibilityScope == ResponsibilityScope.EquipmentOnly &&
            request.Scope == WorkOrderScope.TowerInfrastructure)
        {
            return Result.Failure<WorkOrderDto>("TowerInfrastructure scope is not allowed for equipment-only sites.");
        }

        var (responseMinutes, resolutionMinutes) = await ResolveSlaMinutesAsync(request.SlaClass, cancellationToken);

        var workOrder = WorkOrder.Create(
            request.WoNumber,
            request.SiteCode,
            request.OfficeCode,
            request.SlaClass,
            request.IssueDescription,
            request.Scope,
            responseMinutes,
            resolutionMinutes);

        await _workOrderRepository.AddAsync(workOrder, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _sender.Send(new LogAuditEntryCommand
        {
            EntityType = "WorkOrder",
            EntityId = workOrder.Id,
            Action = "Created",
            ActorId = Guid.Empty,
            ActorRole = "System",
            NewState = workOrder.Status.ToString(),
            Notes = request.IssueDescription
        }, cancellationToken);

        return Result.Success(_mapper.Map<WorkOrderDto>(workOrder));
    }

    private async Task<(int ResponseMinutes, int ResolutionMinutes)> ResolveSlaMinutesAsync(
        SlaClass slaClass,
        CancellationToken cancellationToken)
    {
        var (responseKey, defaultResponse, resolutionKey, defaultResolution) = slaClass switch
        {
            SlaClass.P1 => ("SLA:P1:ResponseMinutes", 60, "SLA:P1:ResolutionMinutes", 240),
            SlaClass.P2 => ("SLA:P2:ResponseMinutes", 240, "SLA:P2:ResolutionMinutes", 480),
            SlaClass.P3 => ("SLA:P3:ResponseMinutes", 1440, "SLA:P3:ResolutionMinutes", 1440),
            SlaClass.P4 => ("SLA:P4:ResponseMinutes", 2880, "SLA:P4:ResolutionMinutes", 2880),
            _ => ("SLA:P3:ResponseMinutes", 1440, "SLA:P3:ResolutionMinutes", 1440)
        };

        var responseMinutes = await _systemSettingsService.GetAsync(responseKey, defaultResponse, cancellationToken);
        var resolutionMinutes = await _systemSettingsService.GetAsync(resolutionKey, defaultResolution, cancellationToken);

        if (responseMinutes <= 0)
            responseMinutes = defaultResponse;

        if (resolutionMinutes <= 0)
            resolutionMinutes = defaultResolution;

        return (responseMinutes, resolutionMinutes);
    }
}
