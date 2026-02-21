using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using TelecomPM.Application.Commands.Auth.Login;
using TelecomPM.Application.Common.Interfaces;
using TelecomPM.Domain.Entities.Users;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Interfaces.Repositories;
using Xunit;

namespace TelecomPM.Application.Tests.Commands.Auth;

public class LoginCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenUserMustChangePassword_ShouldReturnRequiresPasswordChangeFlag()
    {
        var hasher = new PasswordHasher<User>();
        var user = User.Create(
            "Login User",
            "login@example.com",
            "+201000000001",
            UserRole.PMEngineer,
            Guid.NewGuid());
        user.SetPassword("PassWord1A", hasher);
        user.RequirePasswordChange();

        var userRepository = new Mock<IUserRepository>();
        userRepository
            .Setup(r => r.GetByEmailAsNoTrackingAsync("login@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var jwtService = new Mock<IJwtTokenService>();
        jwtService
            .Setup(s => s.GenerateToken(user))
            .Returns(("token", DateTime.UtcNow.AddMinutes(60)));

        var handler = new LoginCommandHandler(
            userRepository.Object,
            jwtService.Object,
            hasher);

        var result = await handler.Handle(new LoginCommand
        {
            Email = "login@example.com",
            Password = "PassWord1A"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.RequiresPasswordChange.Should().BeTrue();
    }
}
