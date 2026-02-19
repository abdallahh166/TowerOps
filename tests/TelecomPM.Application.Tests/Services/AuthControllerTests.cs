using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TelecomPm.Api.Contracts.Auth;
using TelecomPm.Api.Controllers;
using TelecomPM.Api.Services;
using TelecomPM.Domain.Entities.Users;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Interfaces.Repositories;
using Xunit;

namespace TelecomPM.Application.Tests.Services;

public class AuthControllerTests
{
    [Fact]
    public async Task Login_WithValidCredentials_ReturnsToken()
    {
        var user = User.Create("User A", "user@example.com", "01000000000", UserRole.Manager, Guid.NewGuid());

        var repo = new Mock<IUserRepository>();
        repo.Setup(r => r.GetByEmailAsNoTrackingAsync("user@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var tokenService = new Mock<IJwtTokenService>();
        tokenService.Setup(t => t.GenerateToken(user))
            .Returns(("jwt-token", DateTime.UtcNow.AddMinutes(30)));

        var controller = new AuthController(repo.Object, tokenService.Object);

        var response = await controller.Login(new LoginRequest
        {
            Email = "user@example.com",
            PhoneNumber = "01000000000"
        }, CancellationToken.None);

        var ok = response.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task Login_WithWrongPhone_ReturnsUnauthorized()
    {
        var user = User.Create("User A", "user@example.com", "01000000000", UserRole.Manager, Guid.NewGuid());
        var repo = new Mock<IUserRepository>();
        repo.Setup(r => r.GetByEmailAsNoTrackingAsync("user@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var controller = new AuthController(repo.Object, Mock.Of<IJwtTokenService>());

        var response = await controller.Login(new LoginRequest
        {
            Email = "user@example.com",
            PhoneNumber = "01111111111"
        }, CancellationToken.None);

        response.Should().BeOfType<UnauthorizedObjectResult>();
    }
}
