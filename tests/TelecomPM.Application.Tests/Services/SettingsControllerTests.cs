using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TelecomPm.Api.Contracts.Settings;
using TelecomPm.Api.Controllers;
using TelecomPM.Application.Common.Interfaces;
using TelecomPM.Domain.Entities.SystemSettings;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Interfaces.Repositories;
using Xunit;

namespace TelecomPM.Application.Tests.Services;

public class SettingsControllerTests
{
    [Fact]
    public async Task GetAll_ShouldMaskEncryptedValues()
    {
        var settingsRepository = new Mock<ISystemSettingsRepository>();
        settingsRepository
            .Setup(r => r.GetAllAsNoTrackingAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<SystemSetting>
            {
                SystemSetting.Create(
                    "Notifications:Twilio:AuthToken",
                    "encrypted-value",
                    "Notifications",
                    "secret",
                    null,
                    true,
                    "admin"),
                SystemSetting.Create(
                    "SLA:P1:ResponseMinutes",
                    "60",
                    "SLA",
                    "int",
                    null,
                    false,
                    "admin")
            });

        var controller = CreateController(settingsRepository.Object);

        var result = await controller.GetAll(CancellationToken.None);

        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        var grouped = ok.Value.Should().BeAssignableTo<Dictionary<string, List<SystemSettingResponse>>>().Subject;

        grouped["Notifications"][0].Value.Should().Be("***");
        grouped["SLA"][0].Value.Should().Be("60");
    }

    private static SettingsController CreateController(ISystemSettingsRepository settingsRepository)
    {
        var controller = new SettingsController(
            settingsRepository,
            Mock.Of<ISettingsEncryptionService>(),
            Mock.Of<IUnitOfWork>(),
            Mock.Of<INotificationService>(),
            Mock.Of<IEmailService>(),
            Mock.Of<ICurrentUserService>(),
            Mock.Of<IUserRepository>())
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };

        return controller;
    }
}
