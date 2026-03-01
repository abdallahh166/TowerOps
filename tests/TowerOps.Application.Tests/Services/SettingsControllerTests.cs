using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using TowerOps.Api.Contracts.Common;
using TowerOps.Api.Contracts.Settings;
using TowerOps.Api.Controllers;
using TowerOps.Application.Common;
using TowerOps.Application.Common.Interfaces;
using TowerOps.Application.DTOs.Settings;
using TowerOps.Application.Queries.Settings.GetAllSystemSettings;
using TowerOps.Domain.Interfaces.Repositories;
using Xunit;

namespace TowerOps.Application.Tests.Services;

public class SettingsControllerTests
{
    [Fact]
    public async Task GetAll_ShouldReturnMaskedEncryptedValues()
    {
        var sender = new Mock<ISender>();
        sender
            .Setup(s => s.Send(It.IsAny<GetAllSystemSettingsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(new PaginatedList<SystemSettingDto>(
                new List<SystemSettingDto>
                {
                    new()
                    {
                        Key = "Notifications:Twilio:AuthToken",
                        Group = "Notifications",
                        DataType = "secret",
                        IsEncrypted = true,
                        Value = "***",
                        UpdatedAtUtc = DateTime.UtcNow,
                        UpdatedBy = "admin"
                    },
                    new()
                    {
                        Key = "SLA:P1:ResponseMinutes",
                        Group = "SLA",
                        DataType = "int",
                        IsEncrypted = false,
                        Value = "60",
                        UpdatedAtUtc = DateTime.UtcNow,
                        UpdatedBy = "admin"
                    }
                },
                count: 2,
                pageNumber: 1,
                pageSize: 25)));

        var controller = BuildController(sender.Object);

        var result = await controller.GetAll(1, 25, null, "desc", CancellationToken.None);

        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = ok.Value.Should().BeAssignableTo<PagedResponse<SystemSettingResponse>>().Subject;

        response.Data.Should().HaveCount(2);
        response.Data.Single(x => x.Group == "Notifications").Value.Should().Be("***");
        response.Data.Single(x => x.Group == "SLA").Value.Should().Be("60");
        response.Pagination.Total.Should().Be(2);
    }

    private static SettingsController BuildController(ISender sender)
    {
        var services = new ServiceCollection();
        services.AddSingleton(sender);
        var provider = services.BuildServiceProvider();

        var controller = new SettingsController(
            Mock.Of<INotificationService>(),
            Mock.Of<IEmailService>(),
            Mock.Of<ICurrentUserService>(),
            Mock.Of<IUserRepository>())
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
