using Microsoft.EntityFrameworkCore;
using TelecomPM.Domain.Entities.Sync;
using TelecomPM.Domain.Interfaces.Repositories;

namespace TelecomPM.Infrastructure.Persistence.Repositories;

public sealed class SyncQueueRepository : Repository<SyncQueue, Guid>, ISyncQueueRepository
{
    public SyncQueueRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<bool> ExistsDuplicateAsync(
        string deviceId,
        string engineerId,
        string operationType,
        DateTime createdOnDeviceUtc,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .AnyAsync(x =>
                x.DeviceId == deviceId &&
                x.EngineerId == engineerId &&
                x.OperationType == operationType &&
                x.CreatedOnDeviceUtc == createdOnDeviceUtc,
                cancellationToken);
    }

    public async Task<IReadOnlyList<SyncQueue>> GetByDeviceIdAsNoTrackingAsync(string deviceId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(x => x.DeviceId == deviceId)
            .OrderBy(x => x.CreatedOnDeviceUtc)
            .ToListAsync(cancellationToken);
    }
}
