using TowerOps.Domain.Events;

namespace TowerOps.Domain.Events.VisitEvents;

public sealed record SuspiciousCheckInEvent(
    Guid VisitId,
    Guid SiteId,
    Guid EngineerId,
    decimal DistanceFromSiteMeters) : DomainEvent;
