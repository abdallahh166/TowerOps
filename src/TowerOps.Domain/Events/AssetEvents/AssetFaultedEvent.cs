using TowerOps.Domain.Events;

namespace TowerOps.Domain.Events.AssetEvents;

public sealed record AssetFaultedEvent(
    Guid AssetId,
    string AssetCode,
    string SiteCode,
    string? Reason,
    string? EngineerId) : DomainEvent;
