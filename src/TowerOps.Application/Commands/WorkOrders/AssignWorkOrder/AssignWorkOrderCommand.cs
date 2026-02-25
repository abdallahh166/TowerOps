using System;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.WorkOrders;

namespace TowerOps.Application.Commands.WorkOrders.AssignWorkOrder;

public record AssignWorkOrderCommand : ICommand<WorkOrderDto>
{
    public Guid WorkOrderId { get; init; }
    public Guid EngineerId { get; init; }
    public string EngineerName { get; init; } = string.Empty;
    public string AssignedBy { get; init; } = string.Empty;
}
