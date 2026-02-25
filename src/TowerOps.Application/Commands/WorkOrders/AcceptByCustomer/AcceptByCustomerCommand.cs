using TowerOps.Application.Common;
using TowerOps.Application.DTOs.WorkOrders;

namespace TowerOps.Application.Commands.WorkOrders.AcceptByCustomer;

public record AcceptByCustomerCommand : ICommand<WorkOrderDto>
{
    public Guid WorkOrderId { get; init; }
    public string AcceptedBy { get; init; } = string.Empty;
}
