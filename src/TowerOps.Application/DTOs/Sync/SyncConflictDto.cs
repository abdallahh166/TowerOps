namespace TowerOps.Application.DTOs.Sync;

public sealed record SyncConflictDto
{
    public Guid Id { get; init; }
    public Guid SyncQueueId { get; init; }
    public string ConflictType { get; init; } = string.Empty;
    public string Resolution { get; init; } = string.Empty;
    public DateTime ResolvedAtUtc { get; init; }
}
