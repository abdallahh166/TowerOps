using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using TowerOps.Api.Contracts.Roles;
using TowerOps.Api.Controllers;
using TowerOps.Application.Commands.Roles.CreateApplicationRole;
using TowerOps.Application.Commands.Roles.DeleteApplicationRole;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Roles;
using Xunit;

namespace TowerOps.Application.Tests.Services;

public class RolesControllerTests
{
    [Fact]
    public async Task Delete_ShouldRejectSystemRole()
    {
        var sender = new Mock<ISender>();
        sender
            .Setup(s => s.Send(It.IsAny<DeleteApplicationRoleCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure("System roles cannot be deleted."));

        var controller = BuildController(sender.Object);

        var result = await controller.Delete("Admin", CancellationToken.None);

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task CustomRole_CanBeCreatedAndDeleted()
    {
        var sender = new Mock<ISender>();
        sender
            .Setup(s => s.Send(It.IsAny<CreateApplicationRoleCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(new ApplicationRoleDto
            {
                Name = "CustomOps",
                DisplayName = "Custom Ops",
                IsSystem = false,
                IsActive = true,
                Permissions = new List<string> { "sites.view" }
            }));

        sender
            .Setup(s => s.Send(It.IsAny<DeleteApplicationRoleCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        var controller = BuildController(sender.Object);

        var createResult = await controller.Create(new CreateRoleRequest
        {
            Name = "CustomOps",
            DisplayName = "Custom Ops",
            IsActive = true,
            Permissions = new List<string> { "sites.view" }
        }, CancellationToken.None);

        createResult.Should().BeOfType<CreatedAtActionResult>();

        var deleteResult = await controller.Delete("CustomOps", CancellationToken.None);
        deleteResult.Should().BeOfType<NoContentResult>();
    }

    private static RolesController BuildController(ISender sender)
    {
        var services = new ServiceCollection();
        services.AddSingleton(sender);
        var provider = services.BuildServiceProvider();

        var controller = new RolesController
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    RequestServices = provider
                }
            }
        };

        return controller;
    }
}
