using TelecomPM.Domain.Entities.Sites;
using TelecomPM.Domain.Entities.Users;

namespace TelecomPM.Domain.Services;

public interface ISiteAssignmentService
{
    Task<bool> CanAssignSiteToEngineerAsync(Guid engineerId, Guid siteId, CancellationToken cancellationToken = default);
    Task AssignSiteToEngineerAsync(User engineer, Site site, CancellationToken cancellationToken = default);
    Task UnassignSiteFromEngineerAsync(User engineer, Guid siteId, CancellationToken cancellationToken = default);
    Task<List<User>> GetBestEngineersForSiteAsync(Site site, CancellationToken cancellationToken = default);
}
