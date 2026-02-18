using TelecomPM.Domain.Events;

namespace TelecomPM.Domain.Events.VisitEvents;

public sealed record VisitRejectedEvent(
    Guid VisitId,
    Guid SiteId,
    Guid EngineerId,
    string Reason) : DomainEvent;
