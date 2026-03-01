using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using TowerOps.Application.Commands.Auth.Login;
using TowerOps.Application.Common.Interfaces;
using TowerOps.Domain.Entities.RefreshTokens;
using TowerOps.Domain.Entities.Users;
using TowerOps.Domain.Enums;
using TowerOps.Domain.Interfaces.Repositories;
using Xunit;

namespace TowerOps.Application.Tests.Commands.Auth;

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
            .Setup(r => r.GetByEmailAsync("login@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var jwtService = new Mock<IJwtTokenService>();
        jwtService
            .Setup(s => s.GenerateTokenAsync(user, It.IsAny<CancellationToken>()))
            .ReturnsAsync(("token", DateTime.UtcNow.AddMinutes(60)));

        var refreshTokenRepository = new Mock<IRefreshTokenRepository>();
        RefreshToken? persistedRefreshToken = null;
        refreshTokenRepository
            .Setup(r => r.AddAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()))
            .Callback<RefreshToken, CancellationToken>((token, _) => persistedRefreshToken = token)
            .Returns(Task.CompletedTask);

        var refreshTokenService = new Mock<IRefreshTokenService>();
        refreshTokenService.Setup(s => s.GenerateToken()).Returns("refresh-token-value");
        refreshTokenService.Setup(s => s.HashToken("refresh-token-value")).Returns("HASHED-REFRESH");
        refreshTokenService.Setup(s => s.GetRefreshTokenExpiryUtc()).Returns(DateTime.UtcNow.AddDays(7));

        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var mfaService = new Mock<IMfaService>();
        var emailService = new Mock<IEmailService>();

        var handler = new LoginCommandHandler(
            userRepository.Object,
            refreshTokenRepository.Object,
            jwtService.Object,
            refreshTokenService.Object,
            mfaService.Object,
            emailService.Object,
            hasher,
            unitOfWork.Object);

        var result = await handler.Handle(new LoginCommand
        {
            Email = "login@example.com",
            Password = "PassWord1A"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.RequiresPasswordChange.Should().BeTrue();
        result.Value.RefreshToken.Should().Be("refresh-token-value");
        persistedRefreshToken.Should().NotBeNull();
        persistedRefreshToken!.TokenHash.Should().Be("HASHED-REFRESH");
        persistedRefreshToken.UserId.Should().Be(user.Id);

        refreshTokenRepository.Verify(r => r.AddAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
