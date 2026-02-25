using TowerOps.Domain.Events;

namespace TowerOps.Domain.Events.VisitEvents;

public sealed record VisitRejectedEvent(
    Guid VisitId,
    Guid SiteId,
    Guid EngineerId,
    Guid ReviewerId,
    string Reason) : DomainEvent;
