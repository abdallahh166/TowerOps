using TelecomPM.Domain.Events;

namespace TelecomPM.Domain.Events.SiteEvents;

public sealed record SiteCreatedEvent(
    Guid SiteId,
    string SiteCode,
    Guid OfficeId) : DomainEvent;
