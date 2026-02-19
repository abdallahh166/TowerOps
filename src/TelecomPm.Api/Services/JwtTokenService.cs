namespace TelecomPm.Api.Services;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TelecomPM.Application.Common.Interfaces;
using TelecomPM.Domain.Entities.Users;

public sealed class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _configuration;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public (string token, DateTime expiresAtUtc) GenerateToken(User user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secret = jwtSettings["Secret"] ?? throw new InvalidOperationException("JWT secret key is not configured.");
        var issuer = jwtSettings["Issuer"] ?? throw new InvalidOperationException("JWT issuer is not configured.");
        var audience = jwtSettings["Audience"] ?? throw new InvalidOperationException("JWT audience is not configured.");

        var expiryMinutes = 60;
        if (int.TryParse(jwtSettings["ExpiryInMinutes"], out var parsed) && parsed > 0)
        {
            expiryMinutes = parsed;
        }

        var expiresAtUtc = DateTime.UtcNow.AddMinutes(expiryMinutes);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role.ToString()),
            new("OfficeId", user.OfficeId.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: expiresAtUtc,
            signingCredentials: creds);

        var token = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        return (token, expiresAtUtc);
    }
}
