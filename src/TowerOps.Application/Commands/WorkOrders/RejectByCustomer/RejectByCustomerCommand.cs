using TowerOps.Application.Common;
using TowerOps.Application.DTOs.WorkOrders;

namespace TowerOps.Application.Commands.WorkOrders.RejectByCustomer;

public record RejectByCustomerCommand : ICommand<WorkOrderDto>
{
    public Guid WorkOrderId { get; init; }
    public string Reason { get; init; } = string.Empty;
}
