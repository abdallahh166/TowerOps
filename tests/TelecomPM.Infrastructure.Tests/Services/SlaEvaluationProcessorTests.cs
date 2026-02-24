using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TelecomPM.Application.Common.Interfaces;
using TelecomPM.Domain.Entities.WorkOrders;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Interfaces.Repositories;
using TelecomPM.Domain.Services;
using TelecomPM.Infrastructure.Services;
using Xunit;

namespace TelecomPM.Infrastructure.Tests.Services;

public class SlaEvaluationProcessorTests
{
    [Fact]
    public async Task EvaluateBatchAsync_ShouldEvaluateOpenWorkOrders_AndPersistChanges()
    {
        var workOrder1 = WorkOrder.Create("WO-SLA-EVAL-1", "CAI001", "CAI", SlaClass.P1, "Issue");
        var workOrder2 = WorkOrder.Create("WO-SLA-EVAL-2", "CAI002", "CAI", SlaClass.P2, "Issue");

        var workOrderRepository = new Mock<IWorkOrderRepository>();
        workOrderRepository
            .Setup(r => r.GetOpenForSlaEvaluationAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<WorkOrder> { workOrder1, workOrder2 });

        var slaClockService = new Mock<ISlaClockService>();
        slaClockService
            .Setup(s => s.EvaluateStatusAsync(It.IsAny<WorkOrder>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(SlaStatus.OnTime);

        var settingsService = new Mock<ISystemSettingsService>();
        settingsService
            .Setup(s => s.GetAsync("SLA:Evaluation:BatchSize", 200, It.IsAny<CancellationToken>()))
            .ReturnsAsync(2);

        var unitOfWork = new Mock<IUnitOfWork>();

        var sut = new SlaEvaluationProcessor(
            workOrderRepository.Object,
            slaClockService.Object,
            settingsService.Object,
            unitOfWork.Object,
            Mock.Of<ILogger<SlaEvaluationProcessor>>());

        var processed = await sut.EvaluateBatchAsync(CancellationToken.None);

        processed.Should().Be(2);
        slaClockService.Verify(s => s.EvaluateStatusAsync(workOrder1, It.IsAny<CancellationToken>()), Times.Once);
        slaClockService.Verify(s => s.EvaluateStatusAsync(workOrder2, It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task EvaluateBatchAsync_ShouldSkipPersistence_WhenNoOpenWorkOrders()
    {
        var workOrderRepository = new Mock<IWorkOrderRepository>();
        workOrderRepository
            .Setup(r => r.GetOpenForSlaEvaluationAsync(200, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<WorkOrder>());

        var settingsService = new Mock<ISystemSettingsService>();
        settingsService
            .Setup(s => s.GetAsync("SLA:Evaluation:BatchSize", 200, It.IsAny<CancellationToken>()))
            .ReturnsAsync(200);

        var unitOfWork = new Mock<IUnitOfWork>();

        var sut = new SlaEvaluationProcessor(
            workOrderRepository.Object,
            Mock.Of<ISlaClockService>(),
            settingsService.Object,
            unitOfWork.Object,
            Mock.Of<ILogger<SlaEvaluationProcessor>>());

        var processed = await sut.EvaluateBatchAsync(CancellationToken.None);

        processed.Should().Be(0);
        unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
