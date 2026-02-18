using TelecomPM.Domain.Events;

namespace TelecomPM.Domain.Events.VisitEvents;

public sealed record VisitCorrectionRequestedEvent(
    Guid VisitId,
    Guid SiteId,
    Guid EngineerId,
    string CorrectionNotes) : DomainEvent;
