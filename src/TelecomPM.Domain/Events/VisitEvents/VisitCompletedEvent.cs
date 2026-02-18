using TelecomPM.Domain.Events;
using TelecomPM.Domain.ValueObjects;

namespace TelecomPM.Domain.Events.VisitEvents;

public sealed record VisitCompletedEvent(
    Guid VisitId,
    Guid SiteId,
    Guid EngineerId,
    TimeRange Duration) : DomainEvent;