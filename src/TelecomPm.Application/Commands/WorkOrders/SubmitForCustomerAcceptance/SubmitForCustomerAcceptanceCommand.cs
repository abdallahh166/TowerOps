using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.WorkOrders;

namespace TelecomPM.Application.Commands.WorkOrders.SubmitForCustomerAcceptance;

public record SubmitForCustomerAcceptanceCommand : ICommand<WorkOrderDto>
{
    public Guid WorkOrderId { get; init; }
}
