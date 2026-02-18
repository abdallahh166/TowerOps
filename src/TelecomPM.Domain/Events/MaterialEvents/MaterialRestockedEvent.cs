using TelecomPM.Domain.Events;

namespace TelecomPM.Domain.Events.MaterialEvents;

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

