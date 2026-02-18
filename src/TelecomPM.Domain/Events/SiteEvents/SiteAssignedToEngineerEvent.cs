using TelecomPM.Domain.Events;

namespace TelecomPM.Domain.Events.SiteEvents;

public sealed record SiteAssignedToEngineerEvent(
    Guid SiteId,
    Guid EngineerId,
    Guid AssignedBy) : DomainEvent;
