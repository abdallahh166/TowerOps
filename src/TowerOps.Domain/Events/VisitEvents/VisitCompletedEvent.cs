using TowerOps.Domain.Events;
using TowerOps.Domain.ValueObjects;

namespace TowerOps.Domain.Events.VisitEvents;

public sealed record VisitCompletedEvent(
    Guid VisitId,
    Guid SiteId,
    Guid EngineerId,
    TimeRange Duration) : DomainEvent;