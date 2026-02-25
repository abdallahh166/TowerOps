namespace TowerOps.Domain.Events.WorkOrderEvents;

public sealed record WorkOrderRejectedByCustomerEvent(
    Guid WorkOrderId,
    string WoNumber,
    string Reason) : DomainEvent;
