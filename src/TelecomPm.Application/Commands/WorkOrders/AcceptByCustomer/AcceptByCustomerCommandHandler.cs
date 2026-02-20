using AutoMapper;
using MediatR;
using TelecomPM.Application.Commands.AuditLogs.LogAuditEntry;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.WorkOrders;
using TelecomPM.Domain.Interfaces.Repositories;

namespace TelecomPM.Application.Commands.WorkOrders.AcceptByCustomer;

public class AcceptByCustomerCommandHandler : IRequestHandler<AcceptByCustomerCommand, Result<WorkOrderDto>>
{
    private readonly IWorkOrderRepository _workOrderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ISender _sender;

    public AcceptByCustomerCommandHandler(
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

    public async Task<Result<WorkOrderDto>> Handle(AcceptByCustomerCommand request, CancellationToken cancellationToken)
    {
        var workOrder = await _workOrderRepository.GetByIdAsync(request.WorkOrderId, cancellationToken);
        if (workOrder == null)
            return Result.Failure<WorkOrderDto>("Work order not found");

        try
        {
            var previousState = workOrder.Status.ToString();
            workOrder.AcceptByCustomer(request.AcceptedBy);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _sender.Send(new LogAuditEntryCommand
            {
                EntityType = "WorkOrder",
                EntityId = workOrder.Id,
                Action = "AcceptedByCustomer",
                ActorId = Guid.Empty,
                ActorRole = "Customer",
                PreviousState = previousState,
                NewState = workOrder.Status.ToString(),
                Notes = request.AcceptedBy
            }, cancellationToken);

            return Result.Success(_mapper.Map<WorkOrderDto>(workOrder));
        }
        catch (Exception ex)
        {
            return Result.Failure<WorkOrderDto>(ex.Message);
        }
    }
}
