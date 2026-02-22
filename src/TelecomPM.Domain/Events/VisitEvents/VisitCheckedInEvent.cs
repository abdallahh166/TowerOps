using TelecomPM.Domain.Events;

namespace TelecomPM.Domain.Events.VisitEvents;

public sealed record VisitCheckedInEvent(
    Guid VisitId,
    Guid SiteId,
    Guid EngineerId,
    decimal DistanceFromSiteMeters,
    bool IsWithinSiteRadius) : DomainEvent;
