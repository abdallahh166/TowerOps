using TelecomPM.Domain.Events;

namespace TelecomPM.Domain.Events.WorkOrderEvents;

public sealed record WorkOrderClientSignedEvent(
    Guid WorkOrderId,
    string WoNumber,
    string SignerName,
    DateTime SignedAtUtc) : DomainEvent;
