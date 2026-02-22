namespace TelecomPm.Api.Contracts.Sync;

public sealed class SyncBatchRequest
{
    public string DeviceId { get; init; } = string.Empty;
    public string? EngineerId { get; init; }
    public List<SyncBatchItemRequest> Items { get; init; } = new();
}

public sealed class SyncBatchItemRequest
{
    public string OperationType { get; init; } = string.Empty;
    public string Payload { get; init; } = "{}";
    public DateTime CreatedOnDeviceUtc { get; init; }
}
