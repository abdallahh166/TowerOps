using TowerOps.Domain.Events;

namespace TowerOps.Domain.Events.MaterialEvents;

public sealed record MaterialUsedEvent(
    Guid VisitId,
    Guid MaterialId,
    decimal Quantity,
    decimal TotalCost) : DomainEvent;
