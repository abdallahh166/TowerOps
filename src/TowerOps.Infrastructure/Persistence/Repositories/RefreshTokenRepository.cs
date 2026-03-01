using Microsoft.EntityFrameworkCore;
using TowerOps.Domain.Entities.RefreshTokens;
using TowerOps.Domain.Interfaces.Repositories;

namespace TowerOps.Infrastructure.Persistence.Repositories;

public sealed class RefreshTokenRepository : Repository<RefreshToken, Guid>, IRefreshTokenRepository
{
    public RefreshTokenRepository(ApplicationDbContext context) : base(context)
    {
    }

    public Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default)
    {
        var normalized = NormalizeHash(tokenHash);
        return _context.RefreshTokens
            .FirstOrDefaultAsync(x => x.TokenHash == normalized, cancellationToken);
    }

    public Task<RefreshToken?> GetActiveByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default)
    {
        var normalized = NormalizeHash(tokenHash);
        var now = DateTime.UtcNow;
        return _context.RefreshTokens
            .FirstOrDefaultAsync(
                x => x.TokenHash == normalized &&
                     x.RevokedAtUtc == null &&
                     x.ExpiresAtUtc > now,
                cancellationToken);
    }

    public async Task<IReadOnlyList<RefreshToken>> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await _context.RefreshTokens
            .Where(x => x.UserId == userId && x.RevokedAtUtc == null && x.ExpiresAtUtc > now)
            .ToListAsync(cancellationToken);
    }

    public async Task RevokeAllByUserIdAsync(Guid userId, string reason, CancellationToken cancellationToken = default)
    {
        var activeTokens = await GetActiveByUserIdAsync(userId, cancellationToken);
        foreach (var token in activeTokens)
        {
            token.Revoke(reason);
        }
    }

    private static string NormalizeHash(string tokenHash)
    {
        return string.IsNullOrWhiteSpace(tokenHash) ? string.Empty : tokenHash.Trim().ToUpperInvariant();
    }
}
