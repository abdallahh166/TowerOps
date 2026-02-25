using TowerOps.Domain.Events;

namespace TowerOps.Domain.Events.WorkOrderEvents;

public sealed record WorkOrderClientSignedEvent(
    Guid WorkOrderId,
    string WoNumber,
    string SignerName,
    DateTime SignedAtUtc) : DomainEvent;
