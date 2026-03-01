using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using TowerOps.Application.Commands.Auth.Login;
using TowerOps.Application.Common.Interfaces;
using TowerOps.Domain.Entities.Users;
using TowerOps.Domain.Enums;
using TowerOps.Domain.Interfaces.Repositories;
using Xunit;

namespace TowerOps.Application.Tests.Commands.Auth;

public class LoginSecurityPolicyTests
{
    [Fact]
    public async Task Handle_WhenFiveFailedAttempts_ShouldLockTemporarily()
    {
        var passwordHasher = new PasswordHasher<User>();
        var user = User.Create(
            "Engineer User",
            "engineer@example.com",
            "+201000000001",
            UserRole.PMEngineer,
            Guid.NewGuid());
        user.SetPassword("CorrectPass1A", passwordHasher);

        var userRepository = CreateUserRepository(user);
        var handler = CreateHandler(userRepository.Object, passwordHasher, mfaVerifyResult: false);

        TowerOps.Application.Common.Result<TowerOps.Application.DTOs.Auth.AuthTokenDto>? result = null;
        for (var i = 0; i < 5; i++)
        {
            result = await handler.Handle(new LoginCommand
            {
                Email = user.Email,
                Password = "WrongPass1A"
            }, CancellationToken.None);
        }

        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("temporarily locked");
        user.LockoutEndUtc.Should().NotBeNull();
        user.IsManualLockout.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_WhenAdminWithoutMfaSetup_ShouldDenyLogin()
    {
        var passwordHasher = new PasswordHasher<User>();
        var user = User.Create(
            "Admin User",
            "admin.sec@example.com",
            "+201000000001",
            UserRole.Admin,
            Guid.NewGuid());
        user.SetPassword("CorrectPass1A", passwordHasher);

        var userRepository = CreateUserRepository(user);
        var handler = CreateHandler(userRepository.Object, passwordHasher, mfaVerifyResult: false);

        var result = await handler.Handle(new LoginCommand
        {
            Email = user.Email,
            Password = "CorrectPass1A"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("MFA setup is required");
    }

    [Fact]
    public async Task Handle_WhenManagerWithInvalidMfaCode_ShouldDenyLogin()
    {
        var passwordHasher = new PasswordHasher<User>();
        var user = User.Create(
            "Manager User",
            "manager.sec@example.com",
            "+201000000001",
            UserRole.Manager,
            Guid.NewGuid());
        user.SetPassword("CorrectPass1A", passwordHasher);
        user.ConfigureMfa("MFA-SECRET-KEY", enabled: true);

        var userRepository = CreateUserRepository(user);
        var handler = CreateHandler(userRepository.Object, passwordHasher, mfaVerifyResult: false);

        var result = await handler.Handle(new LoginCommand
        {
            Email = user.Email,
            Password = "CorrectPass1A",
            MfaCode = "123456"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Invalid MFA code");
    }

    private static Mock<IUserRepository> CreateUserRepository(User user)
    {
        var userRepository = new Mock<IUserRepository>();
        userRepository
            .Setup(r => r.GetByEmailAsync(user.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        userRepository
            .Setup(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        userRepository
            .Setup(r => r.GetByRoleAsNoTrackingAsync(UserRole.Admin, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<User>());

        return userRepository;
    }

    private static LoginCommandHandler CreateHandler(
        IUserRepository userRepository,
        IPasswordHasher<User> passwordHasher,
        bool mfaVerifyResult)
    {
        var refreshTokenRepository = new Mock<IRefreshTokenRepository>();
        refreshTokenRepository
            .Setup(r => r.AddAsync(It.IsAny<TowerOps.Domain.Entities.RefreshTokens.RefreshToken>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var refreshTokenService = new Mock<IRefreshTokenService>();
        refreshTokenService.Setup(s => s.GenerateToken()).Returns("refresh-token");
        refreshTokenService.Setup(s => s.HashToken("refresh-token")).Returns("HASHED");
        refreshTokenService.Setup(s => s.GetRefreshTokenExpiryUtc()).Returns(DateTime.UtcNow.AddDays(7));

        var jwtService = new Mock<IJwtTokenService>();
        jwtService
            .Setup(s => s.GenerateTokenAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(("access-token", DateTime.UtcNow.AddMinutes(15)));

        var mfaService = new Mock<IMfaService>();
        mfaService
            .Setup(s => s.VerifyCode(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()))
            .Returns(mfaVerifyResult);

        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        return new LoginCommandHandler(
            userRepository,
            refreshTokenRepository.Object,
            jwtService.Object,
            refreshTokenService.Object,
            mfaService.Object,
            Mock.Of<IEmailService>(),
            passwordHasher,
            unitOfWork.Object);
    }
}

