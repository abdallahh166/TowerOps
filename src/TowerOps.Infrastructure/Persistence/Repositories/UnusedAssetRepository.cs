using Microsoft.EntityFrameworkCore;
using TowerOps.Domain.Entities.UnusedAssets;
using TowerOps.Domain.Interfaces.Repositories;

namespace TowerOps.Infrastructure.Persistence.Repositories;

public sealed class UnusedAssetRepository : Repository<UnusedAsset, Guid>, IUnusedAssetRepository
{
    public UnusedAssetRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<UnusedAsset>> GetByVisitIdsAsNoTrackingAsync(
        IReadOnlyCollection<Guid> visitIds,
        CancellationToken cancellationToken = default)
    {
        if (visitIds.Count == 0)
            return Array.Empty<UnusedAsset>();

        return await _dbSet
            .AsNoTracking()
            .Where(x => x.VisitId.HasValue && visitIds.Contains(x.VisitId.Value))
            .OrderBy(x => x.RecordedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<UnusedAsset>> GetBySiteIdAsNoTrackingAsync(
        Guid siteId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(x => x.SiteId == siteId)
            .OrderBy(x => x.RecordedAtUtc)
            .ToListAsync(cancellationToken);
    }
}
