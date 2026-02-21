using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TelecomPm.Api.Contracts.Roles;
using TelecomPm.Api.Controllers;
using TelecomPM.Domain.Entities.ApplicationRoles;
using TelecomPM.Domain.Interfaces.Repositories;
using Xunit;

namespace TelecomPM.Application.Tests.Services;

public class RolesControllerTests
{
    [Fact]
    public async Task Delete_ShouldRejectSystemRole()
    {
        var systemRole = ApplicationRole.Create(
            "Admin",
            "Administrator",
            null,
            isSystem: true,
            isActive: true,
            Array.Empty<string>());

        var repository = new Mock<IApplicationRoleRepository>();
        repository
            .Setup(r => r.GetByIdAsync("Admin", It.IsAny<CancellationToken>()))
            .ReturnsAsync(systemRole);

        var controller = CreateController(repository.Object);

        var result = await controller.Delete("Admin", CancellationToken.None);

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task CustomRole_CanBeCreatedAndDeleted()
    {
        ApplicationRole? createdRole = null;

        var repository = new Mock<IApplicationRoleRepository>();
        repository
            .Setup(r => r.GetByIdAsync("CustomOps", It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => createdRole);
        repository
            .Setup(r => r.AddAsync(It.IsAny<ApplicationRole>(), It.IsAny<CancellationToken>()))
            .Callback<ApplicationRole, CancellationToken>((role, _) => createdRole = role)
            .Returns(Task.CompletedTask);
        repository
            .Setup(r => r.DeleteAsync(It.IsAny<ApplicationRole>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var controller = CreateController(repository.Object, unitOfWork.Object);

        var createResult = await controller.Create(new CreateRoleRequest
        {
            Name = "CustomOps",
            DisplayName = "Custom Ops",
            IsActive = true,
            Permissions = new List<string> { "sites.view" }
        }, CancellationToken.None);

        createResult.Should().BeOfType<CreatedAtActionResult>();
        createdRole.Should().NotBeNull();

        var deleteResult = await controller.Delete("CustomOps", CancellationToken.None);
        deleteResult.Should().BeOfType<NoContentResult>();
    }

    private static RolesController CreateController(
        IApplicationRoleRepository roleRepository,
        IUnitOfWork? unitOfWork = null)
    {
        var controller = new RolesController(roleRepository, unitOfWork ?? Mock.Of<IUnitOfWork>())
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };

        return controller;
    }
}
