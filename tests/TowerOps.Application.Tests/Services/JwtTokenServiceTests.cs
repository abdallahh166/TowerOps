using System.IdentityModel.Tokens.Jwt;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Moq;
using TowerOps.Api.Services;
using TowerOps.Application.Security;
using TowerOps.Domain.Entities.ApplicationRoles;
using TowerOps.Domain.Entities.Users;
using TowerOps.Domain.Enums;
using TowerOps.Domain.Interfaces.Repositories;
using Xunit;

namespace TowerOps.Application.Tests.Services;

public class JwtTokenServiceTests
{
    [Fact]
    public async Task GenerateToken_ShouldIncludePermissionsClaims()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["JwtSettings:Secret"] = "this-is-a-test-secret-with-minimum-length-32",
                ["JwtSettings:Issuer"] = "TowerOps",
                ["JwtSettings:Audience"] = "TowerOps-Users",
                ["JwtSettings:ExpiryInMinutes"] = "60"
            })
            .Build();

        var roleRepository = new Mock<IApplicationRoleRepository>();
        roleRepository
            .Setup(r => r.GetByNameAsync(UserRole.Manager.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApplicationRole.Create(
                UserRole.Manager.ToString(),
                "Business Manager",
                null,
                isSystem: false,
                isActive: true,
                new[] { PermissionConstants.WorkOrdersAssign, PermissionConstants.SitesView }));

        var service = new JwtTokenService(
            configuration,
            roleRepository.Object,
            new MemoryCache(new MemoryCacheOptions()));
        var user = User.Create(
            "Manager User",
            "manager@example.com",
            "+201000000001",
            UserRole.Manager,
            Guid.NewGuid());

        var (token, _) = await service.GenerateTokenAsync(user);

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
        jwt.Claims.Should().Contain(c =>
            c.Type == PermissionConstants.ClaimType &&
            c.Value == PermissionConstants.WorkOrdersAssign);
        jwt.Claims.Should().Contain(c =>
            c.Type == PermissionConstants.ClaimType &&
            c.Value == PermissionConstants.SitesView);
    }
}
