using FluentAssertions;
using Moq;
using TelecomPM.Application.Services;
using TelecomPM.Domain.Entities.Sync;
using TelecomPM.Domain.Entities.Visits;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Interfaces.Repositories;
using Xunit;

namespace TelecomPM.Application.Tests.Services;

public class SyncQueueProcessorTests
{
    [Fact]
    public async Task ProcessAsync_ShouldProcessItemsInChronologicalOrder()
    {
        var visit = CreateVisit();
        var oldItem = SyncQueue.Create("dev-1", "eng-1", "AddReading",
            $"{{\"visitId\":\"{visit.Id}\",\"readingType\":\"BatteryVoltage\"}}",
            DateTime.UtcNow.AddMinutes(-10));
        var newItem = SyncQueue.Create("dev-1", "eng-1", "AddReading",
            $"{{\"visitId\":\"{visit.Id}\",\"readingType\":\"BatteryVoltage\"}}",
            DateTime.UtcNow.AddMinutes(-1));

        var visitRepo = new Mock<IVisitRepository>();
        visitRepo
            .Setup(r => r.GetByIdAsync(visit.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(visit);

        var conflictRepo = new Mock<ISyncConflictRepository>();
        var sut = new SyncQueueProcessor(visitRepo.Object, conflictRepo.Object);

        var result = await sut.ProcessAsync(new[] { newItem, oldItem }, CancellationToken.None);

        result.Processed.Should().Be(1);
        result.Conflicts.Should().Be(1);
        oldItem.Status.Should().Be(SyncStatus.Processed);
        newItem.Status.Should().Be(SyncStatus.Conflict);
    }

    [Fact]
    public async Task ProcessAsync_ShouldRecordConflict_WhenVisitAlreadySubmitted()
    {
        var visit = CreateVisit();
        typeof(Visit).GetProperty(nameof(Visit.Status))!.SetValue(visit, VisitStatus.Submitted);

        var item = SyncQueue.Create(
            "dev-2",
            "eng-2",
            "SubmitChecklist",
            $"{{\"visitId\":\"{visit.Id}\"}}",
            DateTime.UtcNow);

        var visitRepo = new Mock<IVisitRepository>();
        visitRepo
            .Setup(r => r.GetByIdAsync(visit.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(visit);

        var conflictRepo = new Mock<ISyncConflictRepository>();
        var sut = new SyncQueueProcessor(visitRepo.Object, conflictRepo.Object);

        var result = await sut.ProcessAsync(new[] { item }, CancellationToken.None);

        result.Conflicts.Should().Be(1);
        item.Status.Should().Be(SyncStatus.Conflict);
        conflictRepo.Verify(r => r.AddAsync(It.IsAny<SyncConflict>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ProcessAsync_ShouldIncrementRetryCount_WhenItemFails()
    {
        var item = SyncQueue.Create("dev-3", "eng-3", "AddReading", "{bad-json", DateTime.UtcNow);
        var visitRepo = new Mock<IVisitRepository>();
        var conflictRepo = new Mock<ISyncConflictRepository>();
        var sut = new SyncQueueProcessor(visitRepo.Object, conflictRepo.Object);

        var result = await sut.ProcessAsync(new[] { item }, CancellationToken.None);

        result.Failed.Should().Be(1);
        item.Status.Should().Be(SyncStatus.Failed);
        item.RetryCount.Should().Be(1);
    }

    private static Visit CreateVisit()
        => Visit.Create(
            "V-SYNC-001",
            Guid.NewGuid(),
            "CAI001",
            "Site 1",
            Guid.NewGuid(),
            "Engineer A",
            DateTime.UtcNow.AddMinutes(30),
            VisitType.PreventiveMaintenance);
}
