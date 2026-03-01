using TowerOps.Domain.Common;
using TowerOps.Domain.Exceptions;

namespace TowerOps.Domain.Entities.RefreshTokens;

public sealed class RefreshToken : AggregateRoot<Guid>
{
    public Guid UserId { get; private set; }
    public string TokenHash { get; private set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; private set; }
    public DateTime? RevokedAtUtc { get; private set; }
    public string? RevokeReason { get; private set; }
    public Guid? ReplacedByTokenId { get; private set; }

    private RefreshToken() : base()
    {
    }

    private RefreshToken(Guid userId, string tokenHash, DateTime expiresAtUtc)
        : base(Guid.NewGuid())
    {
        UserId = userId;
        TokenHash = tokenHash;
        ExpiresAtUtc = expiresAtUtc;
    }

    public static RefreshToken Create(Guid userId, string tokenHash, DateTime expiresAtUtc)
    {
        if (userId == Guid.Empty)
            throw new DomainException("UserId is required.", "RefreshToken.UserId.Required");

        if (string.IsNullOrWhiteSpace(tokenHash))
            throw new DomainException("Token hash is required.", "RefreshToken.TokenHash.Required");

        if (expiresAtUtc <= DateTime.UtcNow)
            throw new DomainException("Refresh token expiry must be in the future.", "RefreshToken.Expiry.FutureRequired");

        return new RefreshToken(userId, tokenHash.Trim(), expiresAtUtc);
    }

    public bool IsRevoked => RevokedAtUtc.HasValue;
    public bool IsExpired(DateTime nowUtc) => nowUtc >= ExpiresAtUtc;
    public bool IsActive(DateTime nowUtc) => !IsRevoked && !IsExpired(nowUtc);

    public void Revoke(string reason, Guid? replacedByTokenId = null)
    {
        if (IsRevoked)
            return;

        RevokedAtUtc = DateTime.UtcNow;
        RevokeReason = string.IsNullOrWhiteSpace(reason) ? "Revoked" : reason.Trim();
        ReplacedByTokenId = replacedByTokenId;
    }
}
