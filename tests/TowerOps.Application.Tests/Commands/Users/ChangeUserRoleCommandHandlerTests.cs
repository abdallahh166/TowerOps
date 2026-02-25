using AutoMapper;
using FluentAssertions;
using Moq;
using TowerOps.Application.Commands.Users.ChangeUserRole;
using TowerOps.Application.DTOs.Users;
using TowerOps.Domain.Entities.Users;
using TowerOps.Domain.Enums;
using TowerOps.Domain.Interfaces.Repositories;
using Xunit;

namespace TowerOps.Application.Tests.Commands.Users;

public class ChangeUserRoleCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenDemotingEngineer_ShouldClearEngineerProfileSafely()
    {
        var user = User.Create("Eng User", "eng.user@test.com", "01000000000", UserRole.PMEngineer, Guid.NewGuid());
        user.SetEngineerCapacity(3, new List<string> { "Power" });
        user.AssignSite(Guid.NewGuid());

        var userRepository = new Mock<IUserRepository>();
        userRepository
            .Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var officeRepository = new Mock<IOfficeRepository>();
        officeRepository
            .Setup(r => r.GetByIdAsNoTrackingAsync(user.OfficeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TowerOps.Domain.Entities.Offices.Office?)null);

        var unitOfWork = new Mock<IUnitOfWork>();

        var mapper = new Mock<IMapper>();
        mapper
            .Setup(m => m.Map<UserDto>(It.IsAny<User>()))
            .Returns<User>(u => new UserDto
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                Role = u.Role,
                OfficeId = u.OfficeId,
                IsActive = u.IsActive,
                AssignedSitesCount = u.AssignedSiteIds.Count,
                MaxAssignedSites = u.MaxAssignedSites
            });

        var sut = new ChangeUserRoleCommandHandler(
            userRepository.Object,
            officeRepository.Object,
            unitOfWork.Object,
            mapper.Object);

        var result = await sut.Handle(new ChangeUserRoleCommand
        {
            UserId = user.Id,
            NewRole = UserRole.Manager
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        user.Role.Should().Be(UserRole.Manager);
        user.MaxAssignedSites.Should().BeNull();
        user.Specializations.Should().BeEmpty();
        user.AssignedSiteIds.Should().BeEmpty();

        userRepository.Verify(r => r.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ShouldReturnFailure()
    {
        var userRepository = new Mock<IUserRepository>();
        userRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var officeRepository = new Mock<IOfficeRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var mapper = new Mock<IMapper>();

        var sut = new ChangeUserRoleCommandHandler(
            userRepository.Object,
            officeRepository.Object,
            unitOfWork.Object,
            mapper.Object);

        var result = await sut.Handle(new ChangeUserRoleCommand
        {
            UserId = Guid.NewGuid(),
            NewRole = UserRole.Supervisor
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("User not found");
        unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
