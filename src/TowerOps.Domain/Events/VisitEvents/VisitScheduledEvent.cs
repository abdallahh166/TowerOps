using TowerOps.Domain.Events;

namespace TowerOps.Domain.Events.VisitEvents;

public sealed record VisitScheduledEvent(
    Guid VisitId,
    Guid SiteId,
    Guid EngineerId,
    DateTime ScheduledDate) : DomainEvent;
