using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Sync;

namespace TelecomPM.Application.Queries.Sync.GetSyncStatus;

public sealed record GetSyncStatusQuery : IQuery<SyncStatusDto>
{
    public string DeviceId { get; init; } = string.Empty;
}
