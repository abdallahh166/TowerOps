namespace TelecomPM.Domain.Events.WorkOrderEvents;

public sealed record WorkOrderAssignedEvent(
    Guid WorkOrderId,
    Guid EngineerId,
    string EngineerName,
    string AssignedBy,
    DateTime AssignedAtUtc,
    string Status) : DomainEvent;
