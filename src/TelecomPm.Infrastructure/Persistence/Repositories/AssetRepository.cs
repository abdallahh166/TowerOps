using Microsoft.EntityFrameworkCore;
using TelecomPM.Domain.Entities.Assets;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Interfaces.Repositories;

namespace TelecomPM.Infrastructure.Persistence.Repositories;

public sealed class AssetRepository : Repository<Asset, Guid>, IAssetRepository
{
    public AssetRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override async Task<Asset?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(x => x.ServiceHistory)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public override async Task<Asset?> GetByIdAsNoTrackingAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(x => x.ServiceHistory)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<Asset?> GetByAssetCodeAsync(string assetCode, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(x => x.ServiceHistory)
            .FirstOrDefaultAsync(x => x.AssetCode == assetCode, cancellationToken);
    }

    public async Task<Asset?> GetByAssetCodeAsNoTrackingAsync(string assetCode, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(x => x.ServiceHistory)
            .FirstOrDefaultAsync(x => x.AssetCode == assetCode, cancellationToken);
    }

    public async Task<IReadOnlyList<Asset>> GetBySiteCodeAsNoTrackingAsync(string siteCode, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(x => x.ServiceHistory)
            .Where(x => x.SiteCode == siteCode)
            .OrderBy(x => x.AssetCode)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Asset>> GetFaultyAssetsAsNoTrackingAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(x => x.ServiceHistory)
            .Where(x => x.Status == AssetStatus.Faulty)
            .OrderBy(x => x.AssetCode)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Asset>> GetExpiringWarrantiesAsNoTrackingAsync(int days, CancellationToken cancellationToken = default)
    {
        var threshold = DateTime.UtcNow.AddDays(days);
        return await _dbSet
            .AsNoTracking()
            .Include(x => x.ServiceHistory)
            .Where(x => x.WarrantyExpiresAtUtc.HasValue && x.WarrantyExpiresAtUtc.Value <= threshold)
            .OrderBy(x => x.WarrantyExpiresAtUtc)
            .ToListAsync(cancellationToken);
    }
}
