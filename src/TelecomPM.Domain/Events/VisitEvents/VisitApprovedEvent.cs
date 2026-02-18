using TelecomPM.Domain.Events;

namespace TelecomPM.Domain.Events.VisitEvents;

public sealed record VisitApprovedEvent(
    Guid VisitId,
    Guid SiteId,
    Guid EngineerId,
    Guid ReviewerId) : DomainEvent;
