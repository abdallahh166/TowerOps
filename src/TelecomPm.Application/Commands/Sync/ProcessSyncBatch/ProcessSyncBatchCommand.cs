using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Sync;

namespace TelecomPM.Application.Commands.Sync.ProcessSyncBatch;

public sealed record ProcessSyncBatchCommand : ICommand<SyncResultDto>
{
    public string DeviceId { get; init; } = string.Empty;
    public string EngineerId { get; init; } = string.Empty;
    public IReadOnlyList<SyncBatchItem> Items { get; init; } = Array.Empty<SyncBatchItem>();
}

public sealed record SyncBatchItem
{
    public string OperationType { get; init; } = string.Empty;
    public string Payload { get; init; } = "{}";
    public DateTime CreatedOnDeviceUtc { get; init; }
}
