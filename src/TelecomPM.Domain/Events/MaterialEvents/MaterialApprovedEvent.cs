using TelecomPM.Domain.Events;

namespace TelecomPM.Domain.Events.MaterialEvents;

public sealed record MaterialApprovedEvent(
    Guid VisitId,
    Guid MaterialUsageId,
    Guid ApprovedBy) : DomainEvent;
