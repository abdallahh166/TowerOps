using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using TelecomPM.Application.Commands.Users.CreateUser;
using TelecomPM.Application.Common.Interfaces;
using TelecomPM.Application.DTOs.Users;
using TelecomPM.Domain.Entities.Offices;
using TelecomPM.Domain.Entities.Users;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Interfaces.Repositories;
using TelecomPM.Domain.ValueObjects;
using Xunit;

namespace TelecomPM.Application.Tests.Commands.Users;

public class CreateUserInvitationFlowTests
{
    [Fact]
    public async Task Handle_ShouldCreateUserWithMustChangePasswordTrue()
    {
        var office = Office.Create(
            "CAI",
            "Cairo Office",
            "Cairo",
            Address.Create("Street", "Cairo", "Cairo"));

        var userRepository = new Mock<IUserRepository>();
        userRepository
            .Setup(r => r.GetByEmailAsync("new.user@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        User? capturedUser = null;
        userRepository
            .Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Callback<User, CancellationToken>((u, _) => capturedUser = u)
            .Returns(Task.CompletedTask);

        var officeRepository = new Mock<IOfficeRepository>();
        officeRepository
            .Setup(r => r.GetByIdAsync(office.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(office);

        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var mapper = new Mock<IMapper>();
        mapper
            .Setup(m => m.Map<UserDto>(It.IsAny<User>()))
            .Returns((User u) => new UserDto
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                Role = u.Role,
                OfficeId = u.OfficeId,
                IsActive = u.IsActive
            });

        var emailService = new Mock<IEmailService>();

        var handler = new CreateUserCommandHandler(
            userRepository.Object,
            officeRepository.Object,
            unitOfWork.Object,
            mapper.Object,
            new PasswordHasher<User>(),
            emailService.Object);

        var result = await handler.Handle(new CreateUserCommand
        {
            Name = "New User",
            Email = "new.user@example.com",
            PhoneNumber = "+201000000123",
            Password = "IgnoredPass1A!",
            Role = UserRole.PMEngineer,
            OfficeId = office.Id
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        capturedUser.Should().NotBeNull();
        capturedUser!.MustChangePassword.Should().BeTrue();
        emailService.Verify(e => e.SendEmailAsync(
            "new.user@example.com",
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
