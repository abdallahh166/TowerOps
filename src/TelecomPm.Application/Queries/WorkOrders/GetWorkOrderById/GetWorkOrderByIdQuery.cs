using System;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.WorkOrders;

namespace TelecomPM.Application.Queries.WorkOrders.GetWorkOrderById;

public record GetWorkOrderByIdQuery : IQuery<WorkOrderDto>
{
    public Guid WorkOrderId { get; init; }
}
