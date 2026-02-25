using System;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.WorkOrders;

namespace TowerOps.Application.Queries.WorkOrders.GetWorkOrderById;

public record GetWorkOrderByIdQuery : IQuery<WorkOrderDto>
{
    public Guid WorkOrderId { get; init; }
}
