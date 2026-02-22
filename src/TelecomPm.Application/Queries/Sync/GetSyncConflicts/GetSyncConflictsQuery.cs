using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Sync;

namespace TelecomPM.Application.Queries.Sync.GetSyncConflicts;

public sealed record GetSyncConflictsQuery : IQuery<IReadOnlyList<SyncConflictDto>>
{
    public string EngineerId { get; init; } = string.Empty;
}
