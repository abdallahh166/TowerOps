using TelecomPM.Domain.Events;

namespace TelecomPM.Domain.Events.AssetEvents;

public sealed record AssetFaultedEvent(
    Guid AssetId,
    string AssetCode,
    string SiteCode,
    string? Reason,
    string? EngineerId) : DomainEvent;
