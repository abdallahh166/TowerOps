using TelecomPM.Domain.Entities.Sync;

namespace TelecomPM.Domain.Interfaces.Repositories;

public interface ISyncConflictRepository : IRepository<SyncConflict, Guid>
{
    Task<IReadOnlyList<SyncConflict>> GetByEngineerIdAsNoTrackingAsync(string engineerId, CancellationToken cancellationToken = default);
}
