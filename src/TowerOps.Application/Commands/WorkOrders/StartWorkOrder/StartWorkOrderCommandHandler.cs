using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using TowerOps.Application.Common;
using TowerOps.Application.Commands.AuditLogs.LogAuditEntry;
using TowerOps.Application.DTOs.WorkOrders;
using TowerOps.Domain.Interfaces.Repositories;

namespace TowerOps.Application.Commands.WorkOrders.StartWorkOrder;

public class StartWorkOrderCommandHandler : IRequestHandler<StartWorkOrderCommand, Result<WorkOrderDto>>
{
    private readonly IWorkOrderRepository _workOrderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ISender _sender;

    public StartWorkOrderCommandHandler(
        IWorkOrderRepository workOrderRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ISender sender)
    {
        _workOrderRepository = workOrderRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _sender = sender;
    }

    public async Task<Result<WorkOrderDto>> Handle(StartWorkOrderCommand request, CancellationToken cancellationToken)
    {
        var workOrder = await _workOrderRepository.GetByIdAsync(request.WorkOrderId, cancellationToken);
        if (workOrder == null)
            return Result.Failure<WorkOrderDto>("Work order not found");

        try
        {
            var previousState = workOrder.Status.ToString();
            workOrder.Start();
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _sender.Send(new LogAuditEntryCommand
            {
                EntityType = "WorkOrder",
                EntityId = workOrder.Id,
                Action = "Started",
                ActorId = Guid.Empty,
                ActorRole = "System",
                PreviousState = previousState,
                NewState = workOrder.Status.ToString()
            }, cancellationToken);

            return Result.Success(_mapper.Map<WorkOrderDto>(workOrder));
        }
        catch (Exception ex)
        {
            return Result.Failure<WorkOrderDto>(ex.Message);
        }
    }
}
