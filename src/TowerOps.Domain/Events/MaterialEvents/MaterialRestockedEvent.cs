using TowerOps.Domain.Events;

namespace TowerOps.Domain.Events.MaterialEvents;

public sealed record MaterialRestockedEvent(
    Guid MaterialId,
    string MaterialName,
    Guid OfficeId,
    decimal Quantity,
    string Unit,
    decimal PreviousStock,
    decimal NewStock,
    string RestockedBy,
    DateTime RestockedAt
) : DomainEvent;

