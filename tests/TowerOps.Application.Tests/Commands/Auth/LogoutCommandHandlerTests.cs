using FluentAssertions;
using Moq;
using TowerOps.Application.Commands.Auth.Logout;
using TowerOps.Application.Common.Interfaces;
using TowerOps.Domain.Entities.RefreshTokens;
using TowerOps.Domain.Interfaces.Repositories;
using Xunit;

namespace TowerOps.Application.Tests.Commands.Auth;

public class LogoutCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithActiveToken_ShouldRevokeToken()
    {
        var token = RefreshToken.Create(Guid.NewGuid(), "TOKEN-HASH", DateTime.UtcNow.AddDays(1));

        var refreshRepository = new Mock<IRefreshTokenRepository>();
        refreshRepository
            .Setup(r => r.GetActiveByTokenHashAsync("TOKEN-HASH", It.IsAny<CancellationToken>()))
            .ReturnsAsync(token);

        var refreshService = new Mock<IRefreshTokenService>();
        refreshService.Setup(s => s.HashToken("plain-token")).Returns("TOKEN-HASH");

        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new LogoutCommandHandler(
            refreshRepository.Object,
            refreshService.Object,
            unitOfWork.Object);

        var result = await handler.Handle(new LogoutCommand
        {
            RefreshToken = "plain-token"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        token.IsRevoked.Should().BeTrue();
        token.RevokeReason.Should().Be("Logout");
        refreshRepository.Verify(r => r.UpdateAsync(token, It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithUnknownToken_ShouldReturnSuccessWithoutPersistence()
    {
        var refreshRepository = new Mock<IRefreshTokenRepository>();
        refreshRepository
            .Setup(r => r.GetActiveByTokenHashAsync("UNKNOWN", It.IsAny<CancellationToken>()))
            .ReturnsAsync((RefreshToken?)null);

        var refreshService = new Mock<IRefreshTokenService>();
        refreshService.Setup(s => s.HashToken("unknown-token")).Returns("UNKNOWN");

        var handler = new LogoutCommandHandler(
            refreshRepository.Object,
            refreshService.Object,
            Mock.Of<IUnitOfWork>());

        var result = await handler.Handle(new LogoutCommand
        {
            RefreshToken = "unknown-token"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        refreshRepository.Verify(r => r.UpdateAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}

