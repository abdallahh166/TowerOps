using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using TelecomPM.Application.Common.Interfaces;
using TelecomPM.Domain.Entities.WorkOrders;
using TelecomPM.Domain.Enums;
using TelecomPM.Infrastructure.Services;
using Xunit;

namespace TelecomPM.Infrastructure.Tests.Services;

public class SlaClockServiceTests
{
    private readonly Mock<ISystemSettingsService> _settingsServiceMock;
    private readonly SlaClockService _service;

    public SlaClockServiceTests()
    {
        _settingsServiceMock = new Mock<ISystemSettingsService>();
        _settingsServiceMock
            .Setup(s => s.GetAsync("SLA:P1:ResponseMinutes", 60, It.IsAny<CancellationToken>()))
            .ReturnsAsync(60);
        _settingsServiceMock
            .Setup(s => s.GetAsync("SLA:P2:ResponseMinutes", 240, It.IsAny<CancellationToken>()))
            .ReturnsAsync(240);
        _settingsServiceMock
            .Setup(s => s.GetAsync("SLA:P3:ResponseMinutes", 1440, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1440);

        _service = new SlaClockService(
            _settingsServiceMock.Object,
            new MemoryCache(new MemoryCacheOptions()));
    }

    [Theory]
    [InlineData(SlaClass.P1, 59, false)]
    [InlineData(SlaClass.P1, 61, true)]
    [InlineData(SlaClass.P2, 239, false)]
    [InlineData(SlaClass.P2, 241, true)]
    [InlineData(SlaClass.P3, 1439, false)]
    [InlineData(SlaClass.P3, 1441, true)]
    public void IsBreached_ShouldRespectFrozenSlaMatrix(SlaClass slaClass, int responseMinutes, bool expectedBreached)
    {
        var createdAtUtc = DateTime.UtcNow;

        var isBreached = _service.IsBreached(createdAtUtc, responseMinutes, slaClass);

        isBreached.Should().Be(expectedBreached);
    }

    [Fact]
    public void EvaluateStatus_ShouldReturnOnTime_WhenFarFromDeadline()
    {
        var workOrder = WorkOrder.Create("WO-SLA-1", "S-1", "CAI", SlaClass.P1, "Issue");
        SetResponseDeadline(workOrder, DateTime.UtcNow.AddMinutes(50));

        var status = _service.EvaluateStatus(workOrder);

        status.Should().Be(SlaStatus.OnTime);
    }

    [Fact]
    public void EvaluateStatus_ShouldReturnAtRisk_WhenWithinThirtyMinutesToDeadline()
    {
        var workOrder = WorkOrder.Create("WO-SLA-2", "S-2", "CAI", SlaClass.P1, "Issue");
        SetResponseDeadline(workOrder, DateTime.UtcNow.AddMinutes(15));

        var status = _service.EvaluateStatus(workOrder);

        status.Should().Be(SlaStatus.AtRisk);
    }

    [Fact]
    public void EvaluateStatus_ShouldReturnBreached_WhenPastDeadline()
    {
        var workOrder = WorkOrder.Create("WO-SLA-3", "S-3", "CAI", SlaClass.P1, "Issue");
        SetResponseDeadline(workOrder, DateTime.UtcNow.AddMinutes(-1));

        var status = _service.EvaluateStatus(workOrder);

        status.Should().Be(SlaStatus.Breached);
    }

    [Fact]
    public void EvaluateStatus_ShouldReturnOnTime_ForBacklogP4()
    {
        var workOrder = WorkOrder.Create("WO-SLA-4", "S-4", "CAI", SlaClass.P4, "Issue");

        var status = _service.EvaluateStatus(workOrder);

        status.Should().Be(SlaStatus.OnTime);
    }

    [Fact]
    public void CalculateDeadline_ShouldUseSettingsValue_NotHardcoded()
    {
        _settingsServiceMock
            .Setup(s => s.GetAsync("SLA:P1:ResponseMinutes", 60, It.IsAny<CancellationToken>()))
            .ReturnsAsync(90);

        var createdAt = new DateTime(2026, 2, 21, 0, 0, 0, DateTimeKind.Utc);

        var deadline = _service.CalculateDeadline(createdAt, SlaClass.P1);

        deadline.Should().Be(createdAt.AddMinutes(90));
    }

    private static void SetResponseDeadline(WorkOrder workOrder, DateTime responseDeadlineUtc)
    {
        typeof(WorkOrder)
            .GetProperty("ResponseDeadlineUtc")!
            .SetValue(workOrder, responseDeadlineUtc);
    }
}
