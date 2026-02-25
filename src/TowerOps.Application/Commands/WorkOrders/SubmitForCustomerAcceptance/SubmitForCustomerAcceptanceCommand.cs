using TowerOps.Application.Common;
using TowerOps.Application.DTOs.WorkOrders;

namespace TowerOps.Application.Commands.WorkOrders.SubmitForCustomerAcceptance;

public record SubmitForCustomerAcceptanceCommand : ICommand<WorkOrderDto>
{
    public Guid WorkOrderId { get; init; }
}
