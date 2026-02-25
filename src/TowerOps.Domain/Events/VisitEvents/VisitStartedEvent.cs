using TowerOps.Domain.Events;

namespace TowerOps.Domain.Events.VisitEvents;

public sealed record VisitStartedEvent(
    Guid VisitId,
    Guid SiteId,
    Guid EngineerId) : DomainEvent;
