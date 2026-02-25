namespace TowerOps.Domain.Events.WorkOrderEvents;

public sealed record SlaBreachedEvent(
    Guid WorkOrderId,
    string WoNumber,
    DateTime ResolutionDeadlineUtc,
    DateTime BreachedAtUtc) : DomainEvent;
