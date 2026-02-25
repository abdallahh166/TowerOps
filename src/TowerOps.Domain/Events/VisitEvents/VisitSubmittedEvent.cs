using TowerOps.Domain.Events;

namespace TowerOps.Domain.Events.VisitEvents;

public sealed record VisitSubmittedEvent(
    Guid VisitId,
    Guid SiteId,
    Guid EngineerId) : DomainEvent;
