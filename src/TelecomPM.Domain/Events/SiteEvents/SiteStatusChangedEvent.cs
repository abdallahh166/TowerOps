using TelecomPM.Domain.Events;
using TelecomPM.Domain.Enums;

namespace TelecomPM.Domain.Events.SiteEvents;

public sealed record SiteStatusChangedEvent(
    Guid SiteId,
    SiteStatus OldStatus,
    SiteStatus NewStatus) : DomainEvent;
