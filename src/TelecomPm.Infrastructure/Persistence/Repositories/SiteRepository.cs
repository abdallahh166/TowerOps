namespace TelecomPM.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using TelecomPM.Domain.Entities.Sites;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Interfaces.Repositories;

public class SiteRepository : Repository<Site, Guid>, ISiteRepository
{
    public SiteRepository(ApplicationDbContext context) : base(context)
    {
    }

    // ✅ Override base method to include related entities (WITH TRACKING)
    public override async Task<Site?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(s => s.TowerInfo)
            .Include(s => s.PowerSystem)
            .Include(s => s.RadioEquipment)
            .Include(s => s.Transmission)
            .Include(s => s.CoolingSystem)
            .Include(s => s.FireSafety)
            .Include(s => s.SharingInfo)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    // ✅ Override base method to include related entities (WITHOUT TRACKING)
    public override async Task<Site?> GetByIdAsNoTrackingAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(s => s.TowerInfo)
            .Include(s => s.PowerSystem)
            .Include(s => s.RadioEquipment)
            .Include(s => s.Transmission)
            .Include(s => s.CoolingSystem)
            .Include(s => s.FireSafety)
            .Include(s => s.SharingInfo)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    // ✅ WITH TRACKING - For updates
    public async Task<Site?> GetBySiteCodeAsync(
        string siteCode,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(s => s.TowerInfo)
            .Include(s => s.PowerSystem)
            .Include(s => s.RadioEquipment)
            .Include(s => s.Transmission)
            .Include(s => s.CoolingSystem)
            .Include(s => s.FireSafety)
            .Include(s => s.SharingInfo)
            .FirstOrDefaultAsync(s => s.SiteCode.Value == siteCode, cancellationToken);
    }

    public async Task<IReadOnlyList<Site>> GetByOfficeIdAsync(
        Guid officeId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(s => s.OfficeId == officeId)
            .OrderBy(s => s.SiteCode.Value)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Site>> GetByEngineerIdAsync(
        Guid engineerId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(s => s.AssignedEngineerId.HasValue && s.AssignedEngineerId.Value == engineerId)
            .OrderBy(s => s.SiteCode.Value)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Site>> GetByComplexityAsync(
        SiteComplexity complexity,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(s => s.Complexity == complexity)
            .OrderBy(s => s.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Site>> GetByStatusAsync(
        SiteStatus status,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(s => s.Status == status)
            .OrderBy(s => s.SiteCode.Value)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Site>> GetSitesNeedingMaintenanceAsync(
        int daysThreshold,
        CancellationToken cancellationToken = default)
    {
        var thresholdDate = DateTime.UtcNow.AddDays(-daysThreshold);
        return await _dbSet
            .Where(s => s.Status == SiteStatus.OnAir &&
                       (!s.LastVisitDate.HasValue || s.LastVisitDate.Value <= thresholdDate))
            .OrderBy(s => s.LastVisitDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Site>> GetByRegionAsync(
        string region,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(s => s.Region == region)
            .OrderBy(s => s.SiteCode.Value)
            .ToListAsync(cancellationToken);
    }

    // ✅ WITHOUT TRACKING - For display/reports
    public async Task<Site?> GetBySiteCodeAsNoTrackingAsync(
        string siteCode,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(s => s.TowerInfo)
            .Include(s => s.PowerSystem)
            .Include(s => s.RadioEquipment)
            .Include(s => s.Transmission)
            .Include(s => s.CoolingSystem)
            .Include(s => s.FireSafety)
            .Include(s => s.SharingInfo)
            .FirstOrDefaultAsync(s => s.SiteCode.Value == siteCode, cancellationToken);
    }

    public async Task<IReadOnlyList<Site>> GetByOfficeIdAsNoTrackingAsync(
        Guid officeId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(s => s.OfficeId == officeId)
            .OrderBy(s => s.SiteCode.Value)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Site>> GetByEngineerIdAsNoTrackingAsync(
        Guid engineerId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(s => s.AssignedEngineerId.HasValue && s.AssignedEngineerId.Value == engineerId)
            .OrderBy(s => s.SiteCode.Value)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Site>> GetByComplexityAsNoTrackingAsync(
        SiteComplexity complexity,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(s => s.Complexity == complexity)
            .OrderBy(s => s.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Site>> GetByStatusAsNoTrackingAsync(
        SiteStatus status,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(s => s.Status == status)
            .OrderBy(s => s.SiteCode.Value)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Site>> GetSitesNeedingMaintenanceAsNoTrackingAsync(
        int daysThreshold,
        CancellationToken cancellationToken = default)
    {
        var thresholdDate = DateTime.UtcNow.AddDays(-daysThreshold);
        return await _dbSet
            .AsNoTracking()
            .Where(s => s.Status == SiteStatus.OnAir &&
                       (!s.LastVisitDate.HasValue || s.LastVisitDate.Value <= thresholdDate))
            .OrderBy(s => s.LastVisitDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Site>> GetByRegionAsNoTrackingAsync(
        string region,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(s => s.Region == region)
            .OrderBy(s => s.SiteCode.Value)
            .ToListAsync(cancellationToken);
    }

    // ✅ QUERY METHODS - Always AsNoTracking
    public async Task<bool> IsSiteCodeUniqueAsync(
        string siteCode,
        Guid? excludeSiteId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsNoTracking().Where(s => s.SiteCode.Value == siteCode);

        if (excludeSiteId.HasValue)
        {
            query = query.Where(s => s.Id != excludeSiteId.Value);
        }

        return !await query.AnyAsync(cancellationToken);
    }

    public async Task<bool> CodeExistsAsync(
        string siteCode,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .AnyAsync(s => s.SiteCode.Value == siteCode, cancellationToken);
    }

    public async Task<int> GetSiteCountByOfficeAsync(
        Guid officeId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .CountAsync(s => s.OfficeId == officeId, cancellationToken);
    }

    public async Task<int> GetSiteCountByEngineerAsync(
        Guid engineerId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .CountAsync(s => s.AssignedEngineerId.HasValue && s.AssignedEngineerId.Value == engineerId,
                cancellationToken);
    }

    public async Task<int> GetSiteCountByComplexityAsync(
        SiteComplexity complexity,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .CountAsync(s => s.Complexity == complexity, cancellationToken);
    }

    public async Task<int> GetSiteCountByStatusAsync(
        SiteStatus status,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .CountAsync(s => s.Status == status, cancellationToken);
    }

    public async Task<int> GetMaintenanceOverdueCountAsync(
        int daysThreshold,
        CancellationToken cancellationToken = default)
    {
        var thresholdDate = DateTime.UtcNow.AddDays(-daysThreshold);
        return await _dbSet
            .AsNoTracking()
            .CountAsync(s => s.Status == SiteStatus.OnAir &&
                           (!s.LastVisitDate.HasValue || s.LastVisitDate.Value <= thresholdDate),
                       cancellationToken);
    }

    public async Task<int> GetSiteCountByRegionAsync(
        string region,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .CountAsync(s => s.Region == region, cancellationToken);
    }

    public async Task<bool> HasActiveSitesAsync(
        Guid officeId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .AnyAsync(s => s.OfficeId == officeId && s.Status == SiteStatus.OnAir,
                cancellationToken);
    }

    public async Task<IReadOnlyList<Site>> GetUnassignedSitesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(s => !s.AssignedEngineerId.HasValue && s.Status == SiteStatus.OnAir)
            .OrderBy(s => s.SiteCode.Value)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Site>> GetUnassignedSitesAsNoTrackingAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(s => !s.AssignedEngineerId.HasValue && s.Status == SiteStatus.OnAir)
            .OrderBy(s => s.SiteCode.Value)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetUnassignedSitesCountAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .CountAsync(s => !s.AssignedEngineerId.HasValue && s.Status == SiteStatus.OnAir,
                cancellationToken);
    }

    public async Task<IReadOnlyList<Site>> GetBySubRegionAsync(
        string subRegion,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(s => s.SubRegion == subRegion)
            .OrderBy(s => s.SiteCode.Value)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Site>> GetBySubRegionAsNoTrackingAsync(
        string subRegion,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(s => s.SubRegion == subRegion)
            .OrderBy(s => s.SiteCode.Value)
            .ToListAsync(cancellationToken);
    }
}