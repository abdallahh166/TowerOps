using FluentAssertions;
using Moq;
using TowerOps.Application.Services;
using TowerOps.Domain.Entities.Sync;
using TowerOps.Domain.Entities.Visits;
using TowerOps.Domain.Enums;
using TowerOps.Domain.Interfaces.Repositories;
using Xunit;

namespace TowerOps.Application.Tests.Services;

public class SyncQueueProcessorTests
{
    [Fact]
    public async Task ProcessAsync_ShouldProcessItemsInChronologicalOrder()
    {
        var visit = CreateVisit();
        var oldItem = SyncQueue.Create("dev-1", "eng-1", "AddReading",
            $"{{\"visitId\":\"{visit.Id}\",\"readingType\":\"BatteryVoltage\",\"value\":53.2,\"unit\":\"V\"}}",
            DateTime.UtcNow.AddMinutes(-10));
        var newItem = SyncQueue.Create("dev-1", "eng-1", "AddReading",
            $"{{\"visitId\":\"{visit.Id}\",\"readingType\":\"BatteryVoltage\",\"value\":53.3,\"unit\":\"V\"}}",
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

    [Fact]
    public async Task ProcessAsync_AddPhoto_ShouldMutateVisitAndMarkProcessed()
    {
        var visit = CreateVisit();
        var item = SyncQueue.Create(
            "dev-4",
            "eng-4",
            "AddPhoto",
            $"{{\"visitId\":\"{visit.Id}\",\"type\":\"Before\",\"category\":\"Other\",\"itemName\":\"Gate\",\"fileName\":\"gate.jpg\",\"filePath\":\"/photos/gate.jpg\"}}",
            DateTime.UtcNow);

        var visitRepo = new Mock<IVisitRepository>();
        visitRepo
            .Setup(r => r.GetByIdAsync(visit.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(visit);

        var conflictRepo = new Mock<ISyncConflictRepository>();
        var sut = new SyncQueueProcessor(visitRepo.Object, conflictRepo.Object);

        var result = await sut.ProcessAsync(new[] { item }, CancellationToken.None);

        result.Processed.Should().Be(1);
        item.Status.Should().Be(SyncStatus.Processed);
        visit.Photos.Should().HaveCount(1);
    }

    [Fact]
    public async Task ProcessAsync_CheckIn_ShouldRecordCheckInOnVisit()
    {
        var visit = CreateVisit();
        var item = SyncQueue.Create(
            "dev-5",
            "eng-5",
            "CheckIn",
            $"{{\"visitId\":\"{visit.Id}\",\"latitude\":30.1,\"longitude\":31.2,\"distanceFromSiteMeters\":150,\"allowedRadiusMeters\":200}}",
            DateTime.UtcNow);

        var visitRepo = new Mock<IVisitRepository>();
        visitRepo
            .Setup(r => r.GetByIdAsync(visit.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(visit);

        var conflictRepo = new Mock<ISyncConflictRepository>();
        var sut = new SyncQueueProcessor(visitRepo.Object, conflictRepo.Object);

        var result = await sut.ProcessAsync(new[] { item }, CancellationToken.None);

        result.Processed.Should().Be(1);
        visit.CheckInGeoLocation.Should().NotBeNull();
        visit.DistanceFromSiteMeters.Should().Be(150m);
        visit.IsWithinSiteRadius.Should().BeTrue();
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
            VisitType.BM);
}
