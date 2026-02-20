using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.WorkOrders;
using TelecomPM.Domain.Interfaces.Repositories;

namespace TelecomPM.Application.Commands.WorkOrders.CompleteWorkOrder;

public class CompleteWorkOrderCommandHandler : IRequestHandler<CompleteWorkOrderCommand, Result<WorkOrderDto>>
{
    private readonly IWorkOrderRepository _workOrderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CompleteWorkOrderCommandHandler(
        IWorkOrderRepository workOrderRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _workOrderRepository = workOrderRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<WorkOrderDto>> Handle(CompleteWorkOrderCommand request, CancellationToken cancellationToken)
    {
        var workOrder = await _workOrderRepository.GetByIdAsync(request.WorkOrderId, cancellationToken);
        if (workOrder == null)
            return Result.Failure<WorkOrderDto>("Work order not found");

        try
        {
            workOrder.Complete();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success(_mapper.Map<WorkOrderDto>(workOrder));
        }
        catch (Exception ex)
        {
            return Result.Failure<WorkOrderDto>(ex.Message);
        }
    }
}
