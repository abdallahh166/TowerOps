using TelecomPM.Domain.Common;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Exceptions;

namespace TelecomPM.Domain.Entities.Sync;

public sealed class SyncQueue : AggregateRoot<Guid>
{
    public string DeviceId { get; private set; } = string.Empty;
    public string EngineerId { get; private set; } = string.Empty;
    public string OperationType { get; private set; } = string.Empty;
    public string Payload { get; private set; } = string.Empty;
    public DateTime CreatedOnDeviceUtc { get; private set; }
    public DateTime ReceivedAtUtc { get; private set; }
    public SyncStatus Status { get; private set; }
    public string? ConflictReason { get; private set; }
    public int RetryCount { get; private set; }

    private SyncQueue() : base()
    {
    }

    private SyncQueue(
        string deviceId,
        string engineerId,
        string operationType,
        string payload,
        DateTime createdOnDeviceUtc) : base(Guid.NewGuid())
    {
        DeviceId = deviceId;
        EngineerId = engineerId;
        OperationType = operationType;
        Payload = payload;
        CreatedOnDeviceUtc = DateTime.SpecifyKind(createdOnDeviceUtc, DateTimeKind.Utc);
        ReceivedAtUtc = DateTime.UtcNow;
        Status = SyncStatus.Pending;
        RetryCount = 0;
    }

    public static SyncQueue Create(
        string deviceId,
        string engineerId,
        string operationType,
        string payload,
        DateTime createdOnDeviceUtc)
    {
        if (string.IsNullOrWhiteSpace(deviceId))
            throw new DomainException("DeviceId is required.", "SyncQueue.DeviceId.Required");

        if (string.IsNullOrWhiteSpace(engineerId))
            throw new DomainException("EngineerId is required.", "SyncQueue.EngineerId.Required");

        if (string.IsNullOrWhiteSpace(operationType))
            throw new DomainException("OperationType is required.", "SyncQueue.OperationType.Required");

        return new SyncQueue(deviceId.Trim(), engineerId.Trim(), operationType.Trim(), payload ?? string.Empty, createdOnDeviceUtc);
    }

    public void MarkProcessed()
    {
        Status = SyncStatus.Processed;
        ConflictReason = null;
        MarkAsUpdated("SyncProcessor");
    }

    public void MarkConflict(string conflictReason)
    {
        Status = SyncStatus.Conflict;
        ConflictReason = conflictReason;
        MarkAsUpdated("SyncProcessor");
    }

    public void MarkFailed(string reason)
    {
        Status = SyncStatus.Failed;
        ConflictReason = reason;
        RetryCount++;
        MarkAsUpdated("SyncProcessor");
    }
}
