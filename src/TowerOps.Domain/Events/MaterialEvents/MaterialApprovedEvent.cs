using TowerOps.Domain.Events;

namespace TowerOps.Domain.Events.MaterialEvents;

public sealed record MaterialApprovedEvent(
    Guid VisitId,
    Guid MaterialUsageId,
    Guid ApprovedBy) : DomainEvent;
