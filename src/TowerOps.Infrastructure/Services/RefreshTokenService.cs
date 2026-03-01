using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using TowerOps.Application.Common.Interfaces;

namespace TowerOps.Infrastructure.Services;

public sealed class RefreshTokenService : IRefreshTokenService
{
    private readonly IConfiguration _configuration;

    public RefreshTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken()
    {
        Span<byte> randomBytes = stackalloc byte[64];
        RandomNumberGenerator.Fill(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    public string HashToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return string.Empty;

        var bytes = Encoding.UTF8.GetBytes(token.Trim());
        var hash = SHA256.HashData(bytes);
        return Convert.ToHexString(hash);
    }

    public DateTime GetRefreshTokenExpiryUtc()
    {
        var days = _configuration.GetValue<int?>("JwtSettings:RefreshTokenExpiryInDays") ?? 7;
        if (days <= 0)
            days = 7;

        return DateTime.UtcNow.AddDays(days);
    }
}
