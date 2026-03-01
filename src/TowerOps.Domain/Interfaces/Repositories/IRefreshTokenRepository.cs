using TowerOps.Domain.Entities.RefreshTokens;

namespace TowerOps.Domain.Interfaces.Repositories;

public interface IRefreshTokenRepository : IRepository<RefreshToken, Guid>
{
    Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default);
    Task<RefreshToken?> GetActiveByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RefreshToken>> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task RevokeAllByUserIdAsync(Guid userId, string reason, CancellationToken cancellationToken = default);
}
