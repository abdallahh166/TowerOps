using FluentAssertions;
using Moq;
using TowerOps.Application.Commands.Auth.RefreshToken;
using TowerOps.Application.Common.Interfaces;
using TowerOps.Domain.Entities.RefreshTokens;
using TowerOps.Domain.Entities.Users;
using TowerOps.Domain.Enums;
using TowerOps.Domain.Interfaces.Repositories;
using Xunit;

namespace TowerOps.Application.Tests.Commands.Auth;

public class RefreshTokenCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithActiveToken_ShouldRotateRefreshTokenAndReturnAccessToken()
    {
        var user = User.Create(
            "Token User",
            "token.user@example.com",
            "+201000000001",
            UserRole.PMEngineer,
            Guid.NewGuid());

        var oldRefresh = RefreshToken.Create(user.Id, "OLD-HASH", DateTime.UtcNow.AddDays(2));
        var refreshRepository = new Mock<IRefreshTokenRepository>();
        refreshRepository
            .Setup(r => r.GetActiveByTokenHashAsync("OLD-HASH", It.IsAny<CancellationToken>()))
            .ReturnsAsync(oldRefresh);

        RefreshToken? newRefresh = null;
        refreshRepository
            .Setup(r => r.AddAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()))
            .Callback<RefreshToken, CancellationToken>((token, _) => newRefresh = token)
            .Returns(Task.CompletedTask);

        var userRepository = new Mock<IUserRepository>();
        userRepository
            .Setup(r => r.GetByIdAsNoTrackingAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var refreshService = new Mock<IRefreshTokenService>();
        refreshService.Setup(s => s.HashToken("plain-old-token")).Returns("OLD-HASH");
        refreshService.Setup(s => s.GenerateToken()).Returns("plain-new-token");
        refreshService.Setup(s => s.HashToken("plain-new-token")).Returns("NEW-HASH");
        refreshService.Setup(s => s.GetRefreshTokenExpiryUtc()).Returns(DateTime.UtcNow.AddDays(7));

        var jwtService = new Mock<IJwtTokenService>();
        jwtService
            .Setup(s => s.GenerateTokenAsync(user, It.IsAny<CancellationToken>()))
            .ReturnsAsync(("new-access-token", DateTime.UtcNow.AddMinutes(15)));

        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new RefreshTokenCommandHandler(
            refreshRepository.Object,
            userRepository.Object,
            refreshService.Object,
            jwtService.Object,
            unitOfWork.Object);

        var result = await handler.Handle(new RefreshTokenCommand
        {
            RefreshToken = "plain-old-token"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.AccessToken.Should().Be("new-access-token");
        result.Value.RefreshToken.Should().Be("plain-new-token");

        oldRefresh.IsRevoked.Should().BeTrue();
        oldRefresh.RevokeReason.Should().Be("Rotated");
        oldRefresh.ReplacedByTokenId.Should().Be(newRefresh!.Id);
        newRefresh.TokenHash.Should().Be("NEW-HASH");

        refreshRepository.Verify(r => r.UpdateAsync(oldRefresh, It.IsAny<CancellationToken>()), Times.Once);
        refreshRepository.Verify(r => r.AddAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithUnknownToken_ShouldReturnUnauthorized()
    {
        var refreshRepository = new Mock<IRefreshTokenRepository>();
        refreshRepository
            .Setup(r => r.GetActiveByTokenHashAsync("UNKNOWN-HASH", It.IsAny<CancellationToken>()))
            .ReturnsAsync((RefreshToken?)null);

        var refreshService = new Mock<IRefreshTokenService>();
        refreshService.Setup(s => s.HashToken("unknown-token")).Returns("UNKNOWN-HASH");

        var handler = new RefreshTokenCommandHandler(
            refreshRepository.Object,
            Mock.Of<IUserRepository>(),
            refreshService.Object,
            Mock.Of<IJwtTokenService>(),
            Mock.Of<IUnitOfWork>());

        var result = await handler.Handle(new RefreshTokenCommand
        {
            RefreshToken = "unknown-token"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Unauthorized");
    }
}

