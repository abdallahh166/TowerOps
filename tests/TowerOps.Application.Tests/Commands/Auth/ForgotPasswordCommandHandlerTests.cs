using FluentAssertions;
using Moq;
using TowerOps.Application.Commands.Auth.ForgotPassword;
using TowerOps.Application.Common.Interfaces;
using TowerOps.Domain.Entities.Users;
using TowerOps.Domain.Enums;
using TowerOps.Domain.Interfaces.Repositories;
using Xunit;

namespace TowerOps.Application.Tests.Commands.Auth;

public class ForgotPasswordCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldGenerateOtpAndPersistToken_WhenUserExists()
    {
        var user = User.Create(
            "Reset User",
            "reset@example.com",
            "+201000000001",
            UserRole.PMEngineer,
            Guid.NewGuid());

        var userRepository = new Mock<IUserRepository>();
        userRepository
            .Setup(r => r.GetByEmailAsync("reset@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var tokenRepository = new Mock<IPasswordResetTokenRepository>();
        TowerOps.Domain.Entities.PasswordResetTokens.PasswordResetToken? capturedToken = null;
        tokenRepository
            .Setup(r => r.AddAsync(It.IsAny<TowerOps.Domain.Entities.PasswordResetTokens.PasswordResetToken>(), It.IsAny<CancellationToken>()))
            .Callback<TowerOps.Domain.Entities.PasswordResetTokens.PasswordResetToken, CancellationToken>((token, _) => capturedToken = token)
            .Returns(Task.CompletedTask);

        var emailService = new Mock<IEmailService>();
        var otpService = new Mock<IOtpService>();
        otpService.Setup(o => o.GenerateOtp()).Returns("123456");
        otpService.Setup(o => o.HashOtp("123456")).Returns("hashed-otp");

        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new ForgotPasswordCommandHandler(
            userRepository.Object,
            tokenRepository.Object,
            emailService.Object,
            otpService.Object,
            unitOfWork.Object);

        var result = await handler.Handle(new ForgotPasswordCommand { Email = "reset@example.com" }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        capturedToken.Should().NotBeNull();
        capturedToken!.HashedOtp.Should().Be("hashed-otp");
        capturedToken.HashedOtp.Should().NotBe("123456");
        unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
