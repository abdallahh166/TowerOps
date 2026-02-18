using TelecomPM.Domain.Events;

namespace TelecomPM.Domain.Events.OfficeEvents;

public sealed record OfficeCreatedEvent(
    Guid OfficeId,
    string Code,
    string Name,
    string Region
) : DomainEvent;

