using TowerOps.Domain.Events;

namespace TowerOps.Domain.Events.VisitEvents;

public sealed record VisitCheckedOutEvent(
    Guid VisitId,
    Guid SiteId,
    Guid EngineerId) : DomainEvent;
