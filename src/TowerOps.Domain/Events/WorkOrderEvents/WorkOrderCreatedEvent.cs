namespace TowerOps.Domain.Events.WorkOrderEvents;

public sealed record WorkOrderCreatedEvent(
    Guid WorkOrderId,
    string WoNumber,
    string SiteCode,
    string OfficeCode,
    string SlaClass,
    DateTime ResponseDeadlineUtc,
    DateTime ResolutionDeadlineUtc) : DomainEvent;
