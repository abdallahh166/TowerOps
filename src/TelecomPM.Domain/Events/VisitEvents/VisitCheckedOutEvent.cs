using TelecomPM.Domain.Events;

namespace TelecomPM.Domain.Events.VisitEvents;

public sealed record VisitCheckedOutEvent(
    Guid VisitId,
    Guid SiteId,
    Guid EngineerId) : DomainEvent;
