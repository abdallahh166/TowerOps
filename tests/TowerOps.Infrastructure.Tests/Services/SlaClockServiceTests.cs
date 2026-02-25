using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using TowerOps.Application.Common.Interfaces;
using TowerOps.Domain.Entities.WorkOrders;
using TowerOps.Domain.Enums;
using TowerOps.Infrastructure.Services;
using Xunit;

namespace TowerOps.Infrastructure.Tests.Services;

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
    public async Task IsBreached_ShouldRespectFrozenSlaMatrix(SlaClass slaClass, int responseMinutes, bool expectedBreached)
    {
        var createdAtUtc = DateTime.UtcNow;

        var isBreached = await _service.IsBreachedAsync(createdAtUtc, responseMinutes, slaClass);

        isBreached.Should().Be(expectedBreached);
    }

    [Fact]
    public async Task EvaluateStatus_ShouldReturnOnTime_WhenFarFromDeadline()
    {
        var workOrder = WorkOrder.Create("WO-SLA-1", "S-1", "CAI", SlaClass.P1, "Issue");
        SetResponseDeadline(workOrder, DateTime.UtcNow.AddMinutes(50));

        var status = await _service.EvaluateStatusAsync(workOrder);

        status.Should().Be(SlaStatus.OnTime);
    }

    [Fact]
    public async Task EvaluateStatus_ShouldReturnAtRisk_WhenWithinThirtyMinutesToDeadline()
    {
        var workOrder = WorkOrder.Create("WO-SLA-2", "S-2", "CAI", SlaClass.P1, "Issue");
        SetResponseDeadline(workOrder, DateTime.UtcNow.AddMinutes(15));

        var status = await _service.EvaluateStatusAsync(workOrder);

        status.Should().Be(SlaStatus.AtRisk);
    }

    [Fact]
    public async Task EvaluateStatus_ShouldReturnBreached_WhenPastDeadline()
    {
        var workOrder = WorkOrder.Create("WO-SLA-3", "S-3", "CAI", SlaClass.P1, "Issue");
        SetResponseDeadline(workOrder, DateTime.UtcNow.AddMinutes(-1));

        var status = await _service.EvaluateStatusAsync(workOrder);

        status.Should().Be(SlaStatus.Breached);
    }

    [Fact]
    public async Task EvaluateStatus_ShouldReturnOnTime_ForBacklogP4()
    {
        var workOrder = WorkOrder.Create("WO-SLA-4", "S-4", "CAI", SlaClass.P4, "Issue");

        var status = await _service.EvaluateStatusAsync(workOrder);

        status.Should().Be(SlaStatus.OnTime);
    }

    [Fact]
    public async Task EvaluateStatus_ShouldUseStoredDeadline_NotResponseSettingsLookup()
    {
        var strictSettingsMock = new Mock<ISystemSettingsService>(MockBehavior.Strict);
        strictSettingsMock
            .Setup(s => s.GetAsync("SLA:AtRiskThresholdPercent", 70, It.IsAny<CancellationToken>()))
            .ReturnsAsync(70);

        var service = new SlaClockService(
            strictSettingsMock.Object,
            new MemoryCache(new MemoryCacheOptions()));

        var workOrder = WorkOrder.Create("WO-SLA-IMM-1", "S-IMM-1", "CAI", SlaClass.P1, "Issue");
        SetResponseDeadline(workOrder, DateTime.UtcNow.AddMinutes(-1));

        var status = await service.EvaluateStatusAsync(workOrder);

        status.Should().Be(SlaStatus.Breached);
        strictSettingsMock.Verify(
            s => s.GetAsync(
                It.Is<string>(key => key.StartsWith("SLA:P", StringComparison.OrdinalIgnoreCase)),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task CalculateDeadline_ShouldUseSettingsValue_NotHardcoded()
    {
        _settingsServiceMock
            .Setup(s => s.GetAsync("SLA:P1:ResponseMinutes", 60, It.IsAny<CancellationToken>()))
            .ReturnsAsync(90);

        var createdAt = new DateTime(2026, 2, 21, 0, 0, 0, DateTimeKind.Utc);

        var deadline = await _service.CalculateDeadlineAsync(createdAt, SlaClass.P1);

        deadline.Should().Be(createdAt.AddMinutes(90));
    }

    private static void SetResponseDeadline(WorkOrder workOrder, DateTime responseDeadlineUtc)
    {
        typeof(WorkOrder)
            .GetProperty("ResponseDeadlineUtc")!
            .SetValue(workOrder, responseDeadlineUtc);
    }
}
