namespace TelecomPM.Domain.Events.WorkOrderEvents;

public sealed record WorkOrderSubmittedForCustomerAcceptanceEvent(
    Guid WorkOrderId,
    string WoNumber) : DomainEvent;
