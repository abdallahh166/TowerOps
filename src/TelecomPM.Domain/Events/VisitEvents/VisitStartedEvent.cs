using TelecomPM.Domain.Events;

namespace TelecomPM.Domain.Events.VisitEvents;

public sealed record VisitStartedEvent(
    Guid VisitId,
    Guid SiteId,
    Guid EngineerId) : DomainEvent;
