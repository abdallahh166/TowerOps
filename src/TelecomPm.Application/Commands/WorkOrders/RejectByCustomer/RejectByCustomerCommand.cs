using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.WorkOrders;

namespace TelecomPM.Application.Commands.WorkOrders.RejectByCustomer;

public record RejectByCustomerCommand : ICommand<WorkOrderDto>
{
    public Guid WorkOrderId { get; init; }
    public string Reason { get; init; } = string.Empty;
}
