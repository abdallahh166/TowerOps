namespace TelecomPM.Domain.Events.WorkOrderEvents;

public sealed record WorkOrderRejectedByCustomerEvent(
    Guid WorkOrderId,
    string WoNumber,
    string Reason) : DomainEvent;
