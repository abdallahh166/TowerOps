using TowerOps.Domain.Events;

namespace TowerOps.Domain.Events.MaterialEvents;

public sealed record LowStockAlertEvent(
    Guid MaterialId,
    string MaterialName,
    Guid OfficeId,
    decimal CurrentStock,
    decimal MinimumStock) : DomainEvent;