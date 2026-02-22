namespace TelecomPm.Api.Services;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using TelecomPM.Application.Common.Interfaces;
using TelecomPM.Application.Security;
using TelecomPM.Domain.Entities.Users;
using TelecomPM.Domain.Interfaces.Repositories;

public sealed class JwtTokenService : IJwtTokenService
{
    private static readonly TimeSpan PermissionsCacheDuration = TimeSpan.FromMinutes(5);
    private readonly IConfiguration _configuration;
    private readonly IApplicationRoleRepository _applicationRoleRepository;
    private readonly IMemoryCache _memoryCache;

    public JwtTokenService(
        IConfiguration configuration,
        IApplicationRoleRepository applicationRoleRepository,
        IMemoryCache memoryCache)
    {
        _configuration = configuration;
        _applicationRoleRepository = applicationRoleRepository;
        _memoryCache = memoryCache;
    }

    public async Task<(string token, DateTime expiresAtUtc)> GenerateTokenAsync(
        User user,
        CancellationToken cancellationToken = default)
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

        foreach (var permission in await ResolvePermissionsAsync(user.Role.ToString(), cancellationToken))
        {
            claims.Add(new Claim(PermissionConstants.ClaimType, permission));
        }

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

    private async Task<IReadOnlyList<string>> ResolvePermissionsAsync(string roleName, CancellationToken cancellationToken)
    {
        var cacheKey = $"jwt-role-permissions:{roleName.ToLowerInvariant()}";

        var cachedPermissions = await _memoryCache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = PermissionsCacheDuration;
            var role = await _applicationRoleRepository.GetByNameAsync(roleName, cancellationToken);
            if (role is not null && role.Permissions.Count > 0)
            {
                return role.Permissions.ToList();
            }

            return RolePermissionDefaults.GetDefaultPermissions(roleName).ToList();
        });

        return cachedPermissions ?? RolePermissionDefaults.GetDefaultPermissions(roleName);
    }
}
