using TowerOps.Domain.Events;

namespace TowerOps.Domain.Events.OfficeEvents;

public sealed record OfficeCreatedEvent(
    Guid OfficeId,
    string Code,
    string Name,
    string Region
) : DomainEvent;

