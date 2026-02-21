using System.IdentityModel.Tokens.Jwt;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using TelecomPm.Api.Services;
using TelecomPM.Application.Security;
using TelecomPM.Domain.Entities.ApplicationRoles;
using TelecomPM.Domain.Entities.Users;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Interfaces.Repositories;
using Xunit;

namespace TelecomPM.Application.Tests.Services;

public class JwtTokenServiceTests
{
    [Fact]
    public void GenerateToken_ShouldIncludePermissionsClaims()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["JwtSettings:Secret"] = "this-is-a-test-secret-with-minimum-length-32",
                ["JwtSettings:Issuer"] = "TelecomPM",
                ["JwtSettings:Audience"] = "TelecomPM-Users",
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

        var service = new JwtTokenService(configuration, roleRepository.Object);
        var user = User.Create(
            "Manager User",
            "manager@example.com",
            "+201000000001",
            UserRole.Manager,
            Guid.NewGuid());

        var (token, _) = service.GenerateToken(user);

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
        jwt.Claims.Should().Contain(c =>
            c.Type == PermissionConstants.ClaimType &&
            c.Value == PermissionConstants.WorkOrdersAssign);
        jwt.Claims.Should().Contain(c =>
            c.Type == PermissionConstants.ClaimType &&
            c.Value == PermissionConstants.SitesView);
    }
}
