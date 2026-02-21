using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using TelecomPM.Application.Commands.Auth.ChangePassword;
using TelecomPM.Application.Common.Interfaces;
using TelecomPM.Domain.Entities.Users;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Interfaces.Repositories;
using Xunit;

namespace TelecomPM.Application.Tests.Commands.Auth;

public class ChangePasswordCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithWrongCurrentPassword_ShouldFail()
    {
        var passwordHasher = new PasswordHasher<User>();
        var user = User.Create(
            "Current User",
            "current@example.com",
            "+201000000001",
            UserRole.PMEngineer,
            Guid.NewGuid());
        user.SetPassword("CorrectPass1A", passwordHasher);

        var userRepository = new Mock<IUserRepository>();
        userRepository
            .Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var currentUserService = new Mock<ICurrentUserService>();
        currentUserService.SetupGet(c => c.IsAuthenticated).Returns(true);
        currentUserService.SetupGet(c => c.UserId).Returns(user.Id);

        var unitOfWork = new Mock<IUnitOfWork>();

        var handler = new ChangePasswordCommandHandler(
            userRepository.Object,
            currentUserService.Object,
            passwordHasher,
            unitOfWork.Object);

        var result = await handler.Handle(new ChangePasswordCommand
        {
            CurrentPassword = "WrongPass1A",
            NewPassword = "NewPass1A",
            ConfirmPassword = "NewPass1A"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Current password is incorrect");
        unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithValidCurrentPassword_ShouldClearMustChangePassword()
    {
        var passwordHasher = new PasswordHasher<User>();
        var user = User.Create(
            "Current User",
            "current@example.com",
            "+201000000001",
            UserRole.PMEngineer,
            Guid.NewGuid());
        user.SetPassword("CorrectPass1A", passwordHasher);
        user.RequirePasswordChange();

        var userRepository = new Mock<IUserRepository>();
        userRepository
            .Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var currentUserService = new Mock<ICurrentUserService>();
        currentUserService.SetupGet(c => c.IsAuthenticated).Returns(true);
        currentUserService.SetupGet(c => c.UserId).Returns(user.Id);

        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new ChangePasswordCommandHandler(
            userRepository.Object,
            currentUserService.Object,
            passwordHasher,
            unitOfWork.Object);

        var result = await handler.Handle(new ChangePasswordCommand
        {
            CurrentPassword = "CorrectPass1A",
            NewPassword = "NewPass1A",
            ConfirmPassword = "NewPass1A"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        user.MustChangePassword.Should().BeFalse();
    }
}
