using TelecomPM.Domain.Entities.UnusedAssets;

namespace TelecomPM.Domain.Interfaces.Repositories;

public interface IUnusedAssetRepository : IRepository<UnusedAsset, Guid>
{
    Task<IReadOnlyList<UnusedAsset>> GetByVisitIdsAsNoTrackingAsync(
        IReadOnlyCollection<Guid> visitIds,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<UnusedAsset>> GetBySiteIdAsNoTrackingAsync(
        Guid siteId,
        CancellationToken cancellationToken = default);
}
