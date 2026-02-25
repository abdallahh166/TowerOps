namespace TowerOps.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using TowerOps.Domain.Entities.Offices;
using TowerOps.Domain.Interfaces.Repositories;

public class OfficeRepository : Repository<Office, Guid>, IOfficeRepository
{
    public OfficeRepository(ApplicationDbContext context) : base(context)
    {
    }

    // ✅ WITH TRACKING - For updates
    public async Task<Office?> GetByCodeAsync(
        string officeCode,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(o => o.Code == officeCode, cancellationToken);
    }

    public async Task<IReadOnlyList<Office>> GetByRegionAsync(
        string region,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(o => o.Region == region)
            .OrderBy(o => o.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Office>> GetActiveOfficesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(o => o.IsActive)
            .OrderBy(o => o.Name)
            .ToListAsync(cancellationToken);
    }

    // ✅ WITHOUT TRACKING - For display/reports
    public async Task<Office?> GetByCodeAsNoTrackingAsync(
        string officeCode,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Code == officeCode, cancellationToken);
    }

    public async Task<IReadOnlyList<Office>> GetByRegionAsNoTrackingAsync(
        string region,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(o => o.Region == region)
            .OrderBy(o => o.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Office>> GetActiveOfficesAsNoTrackingAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(o => o.IsActive)
            .OrderBy(o => o.Name)
            .ToListAsync(cancellationToken);
    }

    // ✅ QUERY METHODS - Always AsNoTracking
    public async Task<bool> IsOfficeCodeUniqueAsync(
        string officeCode,
        Guid? excludeOfficeId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsNoTracking().Where(o => o.Code == officeCode);

        if (excludeOfficeId.HasValue)
        {
            query = query.Where(o => o.Id != excludeOfficeId.Value);
        }

        return !await query.AnyAsync(cancellationToken);
    }

    public async Task<bool> CodeExistsAsync(
        string officeCode,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .AnyAsync(o => o.Code == officeCode, cancellationToken);
    }

    public async Task<int> GetOfficeCountByRegionAsync(
        string region,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .CountAsync(o => o.Region == region, cancellationToken);
    }

    public async Task<int> GetActiveOfficeCountAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .CountAsync(o => o.IsActive, cancellationToken);
    }

    // Note: HasActiveSitesAsync removed - requires Sites navigation property on Office entity
    // If needed, add navigation property to Office or query via SiteRepository instead

    public async Task<IReadOnlyList<string>> GetAllRegionsAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(o => !string.IsNullOrEmpty(o.Region))
            .Select(o => o.Region)
            .Distinct()
            .OrderBy(r => r)
            .ToListAsync(cancellationToken);
    }
}