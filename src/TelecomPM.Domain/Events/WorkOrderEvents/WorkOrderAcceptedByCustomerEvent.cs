namespace TelecomPM.Domain.Events.WorkOrderEvents;

public sealed record WorkOrderAcceptedByCustomerEvent(
    Guid WorkOrderId,
    string WoNumber,
    string AcceptedBy) : DomainEvent;
