using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using TowerOps.Application.Commands.Auth.VerifyMfaSetup;
using TowerOps.Application.Common.Interfaces;
using TowerOps.Domain.Entities.Users;
using TowerOps.Domain.Enums;
using TowerOps.Domain.Interfaces.Repositories;
using Xunit;

namespace TowerOps.Application.Tests.Commands.Auth;

public class VerifyMfaSetupCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidCode_ShouldEnableMfa()
    {
        var hasher = new PasswordHasher<User>();
        var user = User.Create(
            "Mfa User",
            "mfa.user@example.com",
            "+201000000001",
            UserRole.Manager,
            Guid.NewGuid());
        user.SetPassword("PassWord1A", hasher);
        user.ConfigureMfa("BASE32SECRET", enabled: false);

        var userRepository = new Mock<IUserRepository>();
        userRepository
            .Setup(r => r.GetByEmailAsync(user.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        userRepository
            .Setup(r => r.UpdateAsync(user, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var mfaService = new Mock<IMfaService>();
        mfaService.Setup(s => s.VerifyCode("BASE32SECRET", "123456", It.IsAny<DateTime>())).Returns(true);

        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new VerifyMfaSetupCommandHandler(
            userRepository.Object,
            mfaService.Object,
            hasher,
            unitOfWork.Object);

        var result = await handler.Handle(new VerifyMfaSetupCommand
        {
            Email = user.Email,
            Password = "PassWord1A",
            Code = "123456"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        user.IsMfaEnabled.Should().BeTrue();
    }
}

