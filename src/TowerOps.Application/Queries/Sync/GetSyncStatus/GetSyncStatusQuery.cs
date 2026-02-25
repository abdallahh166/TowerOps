using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Sync;

namespace TowerOps.Application.Queries.Sync.GetSyncStatus;

public sealed record GetSyncStatusQuery : IQuery<SyncStatusDto>
{
    public string DeviceId { get; init; } = string.Empty;
}
