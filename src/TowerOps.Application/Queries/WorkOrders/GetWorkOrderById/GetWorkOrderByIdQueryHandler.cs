using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.WorkOrders;
using TowerOps.Domain.Interfaces.Repositories;

namespace TowerOps.Application.Queries.WorkOrders.GetWorkOrderById;

public class GetWorkOrderByIdQueryHandler : IRequestHandler<GetWorkOrderByIdQuery, Result<WorkOrderDto>>
{
    private readonly IWorkOrderRepository _workOrderRepository;
    private readonly IMapper _mapper;

    public GetWorkOrderByIdQueryHandler(IWorkOrderRepository workOrderRepository, IMapper mapper)
    {
        _workOrderRepository = workOrderRepository;
        _mapper = mapper;
    }

    public async Task<Result<WorkOrderDto>> Handle(GetWorkOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var workOrder = await _workOrderRepository.GetByIdAsNoTrackingAsync(request.WorkOrderId, cancellationToken);
        if (workOrder == null)
            return Result.Failure<WorkOrderDto>("Work order not found");

        return Result.Success(_mapper.Map<WorkOrderDto>(workOrder));
    }
}
