using TowerOps.Application.DTOs.Sync;
using TowerOps.Domain.Entities.Sync;

namespace TowerOps.Application.Services;

public interface ISyncQueueProcessor
{
    Task<SyncResultDto> ProcessAsync(IReadOnlyList<SyncQueue> queuedItems, CancellationToken cancellationToken = default);
}
