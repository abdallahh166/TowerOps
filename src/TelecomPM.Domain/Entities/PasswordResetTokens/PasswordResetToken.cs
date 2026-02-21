using TelecomPM.Domain.Common;
using TelecomPM.Domain.Exceptions;

namespace TelecomPM.Domain.Entities.PasswordResetTokens;

public sealed class PasswordResetToken : AggregateRoot<Guid>
{
    public string Email { get; private set; } = string.Empty;
    public string HashedOtp { get; private set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; private set; }
    public bool IsUsed { get; private set; }

    private PasswordResetToken() : base()
    {
    }

    private PasswordResetToken(
        string email,
        string hashedOtp,
        DateTime expiresAtUtc) : base(Guid.NewGuid())
    {
        Email = email;
        HashedOtp = hashedOtp;
        ExpiresAtUtc = DateTime.SpecifyKind(expiresAtUtc, DateTimeKind.Utc);
        IsUsed = false;
    }

    public static PasswordResetToken Create(
        string email,
        string hashedOtp,
        DateTime expiresAtUtc)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Email is required.");

        if (string.IsNullOrWhiteSpace(hashedOtp))
            throw new DomainException("Hashed OTP is required.");

        if (expiresAtUtc <= DateTime.UtcNow)
            throw new DomainException("Expiry must be in the future.");

        return new PasswordResetToken(email.Trim(), hashedOtp.Trim(), expiresAtUtc);
    }

    public bool IsExpired(DateTime utcNow) => utcNow >= ExpiresAtUtc;

    public void MarkUsed()
    {
        IsUsed = true;
        MarkAsUpdated("System");
    }
}
