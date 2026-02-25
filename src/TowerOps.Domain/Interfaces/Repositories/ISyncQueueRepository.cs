using TowerOps.Domain.Entities.Sync;

namespace TowerOps.Domain.Interfaces.Repositories;

public interface ISyncQueueRepository : IRepository<SyncQueue, Guid>
{
    Task<bool> ExistsDuplicateAsync(
        string deviceId,
        string engineerId,
        string operationType,
        DateTime createdOnDeviceUtc,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<SyncQueue>> GetByDeviceIdAsNoTrackingAsync(string deviceId, CancellationToken cancellationToken = default);
}
