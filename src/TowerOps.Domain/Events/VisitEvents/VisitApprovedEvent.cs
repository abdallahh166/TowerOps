using TowerOps.Domain.Events;

namespace TowerOps.Domain.Events.VisitEvents;

public sealed record VisitApprovedEvent(
    Guid VisitId,
    Guid SiteId,
    Guid EngineerId,
    Guid ReviewerId) : DomainEvent;
