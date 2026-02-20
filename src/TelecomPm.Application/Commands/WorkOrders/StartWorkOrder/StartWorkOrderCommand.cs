using System;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.WorkOrders;

namespace TelecomPM.Application.Commands.WorkOrders.StartWorkOrder;

public record StartWorkOrderCommand : ICommand<WorkOrderDto>
{
    public Guid WorkOrderId { get; init; }
}
