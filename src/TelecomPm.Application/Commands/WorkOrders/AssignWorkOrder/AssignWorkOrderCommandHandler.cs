using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.WorkOrders;
using TelecomPM.Domain.Interfaces.Repositories;

namespace TelecomPM.Application.Commands.WorkOrders.AssignWorkOrder;

public class AssignWorkOrderCommandHandler : IRequestHandler<AssignWorkOrderCommand, Result<WorkOrderDto>>
{
    private readonly IWorkOrderRepository _workOrderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AssignWorkOrderCommandHandler(
        IWorkOrderRepository workOrderRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _workOrderRepository = workOrderRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<WorkOrderDto>> Handle(AssignWorkOrderCommand request, CancellationToken cancellationToken)
    {
        var workOrder = await _workOrderRepository.GetByIdAsync(request.WorkOrderId, cancellationToken);
        if (workOrder == null)
            return Result.Failure<WorkOrderDto>("Work order not found");

        try
        {
            workOrder.Assign(request.EngineerId, request.EngineerName, request.AssignedBy);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success(_mapper.Map<WorkOrderDto>(workOrder));
        }
        catch (Exception ex)
        {
            return Result.Failure<WorkOrderDto>(ex.Message);
        }
    }
}
