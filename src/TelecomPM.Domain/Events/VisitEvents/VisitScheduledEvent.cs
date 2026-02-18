using TelecomPM.Domain.Events;

namespace TelecomPM.Domain.Events.VisitEvents;

public sealed record VisitScheduledEvent(
    Guid VisitId,
    Guid SiteId,
    Guid EngineerId,
    DateTime ScheduledDate) : DomainEvent;
