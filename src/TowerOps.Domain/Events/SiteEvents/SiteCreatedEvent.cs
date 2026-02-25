using TowerOps.Domain.Events;

namespace TowerOps.Domain.Events.SiteEvents;

public sealed record SiteCreatedEvent(
    Guid SiteId,
    string SiteCode,
    Guid OfficeId) : DomainEvent;
