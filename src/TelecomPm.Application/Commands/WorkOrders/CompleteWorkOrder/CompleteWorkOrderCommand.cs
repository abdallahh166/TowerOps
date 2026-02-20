using System;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.WorkOrders;

namespace TelecomPM.Application.Commands.WorkOrders.CompleteWorkOrder;

public record CompleteWorkOrderCommand : ICommand<WorkOrderDto>
{
    public Guid WorkOrderId { get; init; }
}
