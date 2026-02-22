using TelecomPM.Domain.Enums;

namespace TelecomPM.Application.DTOs.Sync;

public sealed record SyncStatusDto
{
    public string DeviceId { get; init; } = string.Empty;
    public int Total { get; init; }
    public int Pending { get; init; }
    public int Processed { get; init; }
    public int Conflicts { get; init; }
    public int Failed { get; init; }
    public IReadOnlyList<SyncStatusItemDto> Items { get; init; } = Array.Empty<SyncStatusItemDto>();
}

public sealed record SyncStatusItemDto
{
    public Guid Id { get; init; }
    public string OperationType { get; init; } = string.Empty;
    public DateTime CreatedOnDeviceUtc { get; init; }
    public SyncStatus Status { get; init; }
    public string? ConflictReason { get; init; }
    public int RetryCount { get; init; }
}
