using TowerOps.Domain.Entities.Assets;

namespace TowerOps.Domain.Interfaces.Repositories;

public interface IAssetRepository : IRepository<Asset, Guid>
{
    Task<Asset?> GetByAssetCodeAsync(string assetCode, CancellationToken cancellationToken = default);
    Task<Asset?> GetByAssetCodeAsNoTrackingAsync(string assetCode, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Asset>> GetBySiteCodeAsNoTrackingAsync(string siteCode, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Asset>> GetFaultyAssetsAsNoTrackingAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Asset>> GetExpiringWarrantiesAsNoTrackingAsync(int days, CancellationToken cancellationToken = default);
}
