using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.WorkOrders;

namespace TelecomPM.Application.Commands.WorkOrders.AcceptByCustomer;

public record AcceptByCustomerCommand : ICommand<WorkOrderDto>
{
    public Guid WorkOrderId { get; init; }
    public string AcceptedBy { get; init; } = string.Empty;
}
