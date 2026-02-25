using TowerOps.Domain.Events;
using TowerOps.Domain.Enums;

namespace TowerOps.Domain.Events.SiteEvents;

public sealed record SiteStatusChangedEvent(
    Guid SiteId,
    SiteStatus OldStatus,
    SiteStatus NewStatus) : DomainEvent;
