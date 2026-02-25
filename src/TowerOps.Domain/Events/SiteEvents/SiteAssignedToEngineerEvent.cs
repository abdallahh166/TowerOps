using TowerOps.Domain.Events;

namespace TowerOps.Domain.Events.SiteEvents;

public sealed record SiteAssignedToEngineerEvent(
    Guid SiteId,
    Guid EngineerId,
    Guid AssignedBy) : DomainEvent;
