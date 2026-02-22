using TelecomPM.Domain.Events;

namespace TelecomPM.Domain.Events.VisitEvents;

public sealed record SuspiciousCheckInEvent(
    Guid VisitId,
    Guid SiteId,
    Guid EngineerId,
    decimal DistanceFromSiteMeters) : DomainEvent;
