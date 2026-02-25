using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using TowerOps.Application.Commands.Auth.ResetPassword;
using TowerOps.Application.Common.Interfaces;
using TowerOps.Domain.Entities.PasswordResetTokens;
using TowerOps.Domain.Entities.Users;
using TowerOps.Domain.Enums;
using TowerOps.Domain.Interfaces.Repositories;
using Xunit;

namespace TowerOps.Application.Tests.Commands.Auth;

public class ResetPasswordCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidOtp_ShouldResetPasswordAndMarkTokenUsed()
    {
        var passwordHasher = new PasswordHasher<User>();
        var user = User.Create(
            "Reset User",
            "reset@example.com",
            "+201000000001",
            UserRole.PMEngineer,
            Guid.NewGuid());
        user.SetPassword("OldPass1A", passwordHasher);

        var token = PasswordResetToken.Create(
            "reset@example.com",
            "hashed-otp",
            DateTime.UtcNow.AddMinutes(10));

        var userRepository = new Mock<IUserRepository>();
        userRepository.Setup(r => r.GetByEmailAsync("reset@example.com", It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var tokenRepository = new Mock<IPasswordResetTokenRepository>();
        tokenRepository.Setup(r => r.GetLatestByEmailAsync("reset@example.com", It.IsAny<CancellationToken>())).ReturnsAsync(token);

        var otpService = new Mock<IOtpService>();
        otpService.Setup(s => s.VerifyOtp("123456", "hashed-otp")).Returns(true);

        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new ResetPasswordCommandHandler(
            userRepository.Object,
            tokenRepository.Object,
            otpService.Object,
            passwordHasher,
            unitOfWork.Object);

        var result = await handler.Handle(new ResetPasswordCommand
        {
            Email = "reset@example.com",
            Otp = "123456",
            NewPassword = "NewPass1A"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        token.IsUsed.Should().BeTrue();
        user.VerifyPassword("NewPass1A", passwordHasher).Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WithExpiredOtp_ShouldFail()
    {
        var passwordHasher = new PasswordHasher<User>();
        var user = User.Create("Reset User", "reset@example.com", "+201000000001", UserRole.PMEngineer, Guid.NewGuid());
        user.SetPassword("OldPass1A", passwordHasher);

        var token = PasswordResetToken.Create("reset@example.com", "hashed-otp", DateTime.UtcNow.AddMinutes(1));
        typeof(PasswordResetToken).GetProperty(nameof(PasswordResetToken.ExpiresAtUtc))!.SetValue(token, DateTime.UtcNow.AddMinutes(-1));

        var userRepository = new Mock<IUserRepository>();
        userRepository.Setup(r => r.GetByEmailAsync("reset@example.com", It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var tokenRepository = new Mock<IPasswordResetTokenRepository>();
        tokenRepository.Setup(r => r.GetLatestByEmailAsync("reset@example.com", It.IsAny<CancellationToken>())).ReturnsAsync(token);

        var handler = new ResetPasswordCommandHandler(
            userRepository.Object,
            tokenRepository.Object,
            Mock.Of<IOtpService>(),
            passwordHasher,
            Mock.Of<IUnitOfWork>());

        var result = await handler.Handle(new ResetPasswordCommand
        {
            Email = "reset@example.com",
            Otp = "123456",
            NewPassword = "NewPass1A"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Invalid or expired OTP");
    }

    [Fact]
    public async Task Handle_WithWrongOtp_ShouldFail()
    {
        var passwordHasher = new PasswordHasher<User>();
        var user = User.Create("Reset User", "reset@example.com", "+201000000001", UserRole.PMEngineer, Guid.NewGuid());
        user.SetPassword("OldPass1A", passwordHasher);

        var token = PasswordResetToken.Create("reset@example.com", "hashed-otp", DateTime.UtcNow.AddMinutes(10));

        var userRepository = new Mock<IUserRepository>();
        userRepository.Setup(r => r.GetByEmailAsync("reset@example.com", It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var tokenRepository = new Mock<IPasswordResetTokenRepository>();
        tokenRepository.Setup(r => r.GetLatestByEmailAsync("reset@example.com", It.IsAny<CancellationToken>())).ReturnsAsync(token);

        var otpService = new Mock<IOtpService>();
        otpService.Setup(s => s.VerifyOtp("123456", "hashed-otp")).Returns(false);

        var handler = new ResetPasswordCommandHandler(
            userRepository.Object,
            tokenRepository.Object,
            otpService.Object,
            passwordHasher,
            Mock.Of<IUnitOfWork>());

        var result = await handler.Handle(new ResetPasswordCommand
        {
            Email = "reset@example.com",
            Otp = "123456",
            NewPassword = "NewPass1A"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Invalid or expired OTP");
    }
}
