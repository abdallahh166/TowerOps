using TelecomPM.Domain.Entities.Sites;
using TelecomPM.Domain.Enums;

namespace TelecomPM.Domain.Interfaces.Repositories;

public interface ISiteRepository : IRepository<Site, Guid>
{
    // ✅ WITH TRACKING - For updates
    Task<Site?> GetBySiteCodeAsync(string siteCode, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Site>> GetByOfficeIdAsync(Guid officeId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Site>> GetByEngineerIdAsync(Guid engineerId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Site>> GetByComplexityAsync(SiteComplexity complexity, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Site>> GetByStatusAsync(SiteStatus status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Site>> GetSitesNeedingMaintenanceAsync(int daysThreshold, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Site>> GetByRegionAsync(string region, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Site>> GetBySubRegionAsync(string subRegion, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Site>> GetUnassignedSitesAsync(CancellationToken cancellationToken = default);

    // ✅ WITHOUT TRACKING - For display/reports
    Task<Site?> GetBySiteCodeAsNoTrackingAsync(string siteCode, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Site>> GetByOfficeIdAsNoTrackingAsync(Guid officeId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Site>> GetByEngineerIdAsNoTrackingAsync(Guid engineerId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Site>> GetByComplexityAsNoTrackingAsync(SiteComplexity complexity, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Site>> GetByStatusAsNoTrackingAsync(SiteStatus status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Site>> GetSitesNeedingMaintenanceAsNoTrackingAsync(int daysThreshold, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Site>> GetByRegionAsNoTrackingAsync(string region, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Site>> GetBySubRegionAsNoTrackingAsync(string subRegion, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Site>> GetUnassignedSitesAsNoTrackingAsync(CancellationToken cancellationToken = default);

    // ✅ QUERY METHODS - Always optimized
    Task<bool> IsSiteCodeUniqueAsync(string siteCode, Guid? excludeSiteId = null, CancellationToken cancellationToken = default);
    Task<bool> CodeExistsAsync(string siteCode, CancellationToken cancellationToken = default);
    Task<int> GetSiteCountByOfficeAsync(Guid officeId, CancellationToken cancellationToken = default);
    Task<int> GetSiteCountByEngineerAsync(Guid engineerId, CancellationToken cancellationToken = default);
    Task<int> GetSiteCountByComplexityAsync(SiteComplexity complexity, CancellationToken cancellationToken = default);
    Task<int> GetSiteCountByStatusAsync(SiteStatus status, CancellationToken cancellationToken = default);
    Task<int> GetMaintenanceOverdueCountAsync(int daysThreshold, CancellationToken cancellationToken = default);
    Task<int> GetSiteCountByRegionAsync(string region, CancellationToken cancellationToken = default);
    Task<int> GetUnassignedSitesCountAsync(CancellationToken cancellationToken = default);
    Task<bool> HasActiveSitesAsync(Guid officeId, CancellationToken cancellationToken = default);
}