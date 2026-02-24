using TelecomPM.Domain.Common;
using TelecomPM.Domain.Exceptions;

namespace TelecomPM.Domain.Entities.Sync;

public sealed class SyncConflict : AggregateRoot<Guid>
{
    public Guid SyncQueueId { get; private set; }
    public string ConflictType { get; private set; } = string.Empty;
    public string Resolution { get; private set; } = string.Empty;
    public DateTime ResolvedAtUtc { get; private set; }

    private SyncConflict() : base()
    {
    }

    private SyncConflict(
        Guid syncQueueId,
        string conflictType,
        string resolution) : base(Guid.NewGuid())
    {
        SyncQueueId = syncQueueId;
        ConflictType = conflictType;
        Resolution = resolution;
        ResolvedAtUtc = DateTime.UtcNow;
    }

    public static SyncConflict Create(
        Guid syncQueueId,
        string conflictType,
        string resolution)
    {
        if (syncQueueId == Guid.Empty)
            throw new DomainException("SyncQueueId is required.", "SyncConflict.SyncQueueId.Required");

        if (string.IsNullOrWhiteSpace(conflictType))
            throw new DomainException("ConflictType is required.", "SyncConflict.ConflictType.Required");

        if (string.IsNullOrWhiteSpace(resolution))
            throw new DomainException("Resolution is required.", "SyncConflict.Resolution.Required");

        return new SyncConflict(syncQueueId, conflictType.Trim(), resolution.Trim());
    }
}
