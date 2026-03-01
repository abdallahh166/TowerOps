using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using TowerOps.Application.Commands.Auth.GenerateMfaSetup;
using TowerOps.Application.Common.Interfaces;
using TowerOps.Domain.Entities.Users;
using TowerOps.Domain.Enums;
using TowerOps.Domain.Interfaces.Repositories;
using Xunit;

namespace TowerOps.Application.Tests.Commands.Auth;

public class GenerateMfaSetupCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidCredentials_ShouldPersistSecretAndReturnOtpUri()
    {
        var hasher = new PasswordHasher<User>();
        var user = User.Create(
            "Mfa User",
            "mfa.user@example.com",
            "+201000000001",
            UserRole.Manager,
            Guid.NewGuid());
        user.SetPassword("PassWord1A", hasher);

        var userRepository = new Mock<IUserRepository>();
        userRepository
            .Setup(r => r.GetByEmailAsync(user.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        userRepository
            .Setup(r => r.UpdateAsync(user, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var mfaService = new Mock<IMfaService>();
        mfaService.Setup(s => s.GenerateSecret()).Returns("BASE32SECRET");
        mfaService.Setup(s => s.BuildOtpAuthUri(user.Email, "TowerOps", "BASE32SECRET"))
            .Returns("otpauth://totp/TowerOps:mfa.user@example.com?secret=BASE32SECRET");

        var settingsService = new Mock<ISystemSettingsService>();
        settingsService.Setup(s => s.GetAsync("Company:Name", "TowerOps", It.IsAny<CancellationToken>()))
            .ReturnsAsync("TowerOps");

        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new GenerateMfaSetupCommandHandler(
            userRepository.Object,
            mfaService.Object,
            settingsService.Object,
            hasher,
            unitOfWork.Object);

        var result = await handler.Handle(new GenerateMfaSetupCommand
        {
            Email = user.Email,
            Password = "PassWord1A"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Secret.Should().Be("BASE32SECRET");
        result.Value.OtpAuthUri.Should().Contain("otpauth://totp/");
        user.IsMfaEnabled.Should().BeFalse();
        user.MfaSecret.Should().Be("BASE32SECRET");
    }
}

