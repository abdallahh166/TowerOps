using TelecomPM.Domain.Events;

namespace TelecomPM.Domain.Events.UserEvents;

public sealed record EngineerAssignmentChangedEvent(
    Guid EngineerId,
    string EngineerName,
    Guid SiteId,
    string SiteCode,
    string SiteName,
    bool IsAssigned,
    DateTime ChangedAt
) : DomainEvent;

