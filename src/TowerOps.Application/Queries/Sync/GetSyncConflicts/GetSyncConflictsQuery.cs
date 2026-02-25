using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Sync;

namespace TowerOps.Application.Queries.Sync.GetSyncConflicts;

public sealed record GetSyncConflictsQuery : IQuery<IReadOnlyList<SyncConflictDto>>
{
    public string EngineerId { get; init; } = string.Empty;
}
