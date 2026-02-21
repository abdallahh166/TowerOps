namespace TelecomPm.Api.Services;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TelecomPM.Application.Common.Interfaces;
using TelecomPM.Application.Security;
using TelecomPM.Domain.Entities.Users;
using TelecomPM.Domain.Interfaces.Repositories;

public sealed class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _configuration;
    private readonly IApplicationRoleRepository _applicationRoleRepository;

    public JwtTokenService(
        IConfiguration configuration,
        IApplicationRoleRepository applicationRoleRepository)
    {
        _configuration = configuration;
        _applicationRoleRepository = applicationRoleRepository;
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

        foreach (var permission in ResolvePermissions(user.Role.ToString()))
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

    private IReadOnlyList<string> ResolvePermissions(string roleName)
    {
        var role = _applicationRoleRepository
            .GetByNameAsync(roleName)
            .GetAwaiter()
            .GetResult();

        if (role is not null && role.Permissions.Count > 0)
        {
            return role.Permissions.ToList();
        }

        return RolePermissionDefaults.GetDefaultPermissions(roleName);
    }
}
