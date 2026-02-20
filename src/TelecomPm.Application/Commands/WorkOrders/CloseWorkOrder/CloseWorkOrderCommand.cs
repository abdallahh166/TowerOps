using System;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.WorkOrders;

namespace TelecomPM.Application.Commands.WorkOrders.CloseWorkOrder;

public record CloseWorkOrderCommand : ICommand<WorkOrderDto>
{
    public Guid WorkOrderId { get; init; }
}
