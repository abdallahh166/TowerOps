using TowerOps.Domain.Entities.Sync;

namespace TowerOps.Domain.Interfaces.Repositories;

public interface ISyncConflictRepository : IRepository<SyncConflict, Guid>
{
    Task<IReadOnlyList<SyncConflict>> GetByEngineerIdAsNoTrackingAsync(string engineerId, CancellationToken cancellationToken = default);
}
