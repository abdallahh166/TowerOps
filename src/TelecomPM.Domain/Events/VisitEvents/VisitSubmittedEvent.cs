using TelecomPM.Domain.Events;

namespace TelecomPM.Domain.Events.VisitEvents;

public sealed record VisitSubmittedEvent(
    Guid VisitId,
    Guid SiteId,
    Guid EngineerId) : DomainEvent;
