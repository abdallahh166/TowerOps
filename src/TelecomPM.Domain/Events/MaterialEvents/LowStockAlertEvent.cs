using TelecomPM.Domain.Events;

namespace TelecomPM.Domain.Events.MaterialEvents;

public sealed record LowStockAlertEvent(
    Guid MaterialId,
    string MaterialName,
    Guid OfficeId,
    decimal CurrentStock,
    decimal MinimumStock) : DomainEvent;