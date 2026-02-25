using Microsoft.EntityFrameworkCore;
using TowerOps.Domain.Entities.Sync;
using TowerOps.Domain.Interfaces.Repositories;

namespace TowerOps.Infrastructure.Persistence.Repositories;

public sealed class SyncConflictRepository : Repository<SyncConflict, Guid>, ISyncConflictRepository
{
    private readonly ApplicationDbContext _dbContext;

    public SyncConflictRepository(ApplicationDbContext context) : base(context)
    {
        _dbContext = context;
    }

    public async Task<IReadOnlyList<SyncConflict>> GetByEngineerIdAsNoTrackingAsync(string engineerId, CancellationToken cancellationToken = default)
    {
        return await (
            from conflict in _dbContext.SyncConflicts.AsNoTracking()
            join queue in _dbContext.SyncQueues.AsNoTracking() on conflict.SyncQueueId equals queue.Id
            where queue.EngineerId == engineerId
            orderby queue.CreatedOnDeviceUtc descending
            select conflict
        ).ToListAsync(cancellationToken);
    }
}
