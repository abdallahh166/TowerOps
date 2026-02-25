using FluentAssertions;
using Moq;
using TowerOps.Application.Commands.Sync.ProcessSyncBatch;
using TowerOps.Application.Common.Interfaces;
using TowerOps.Application.DTOs.Sync;
using TowerOps.Application.Services;
using TowerOps.Domain.Entities.Sync;
using TowerOps.Domain.Interfaces.Repositories;
using Xunit;

namespace TowerOps.Application.Tests.Commands.Sync;

public class ProcessSyncBatchCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldSkipDuplicateItem()
    {
        var duplicateTime = DateTime.UtcNow.AddMinutes(-5);
        var freshTime = DateTime.UtcNow.AddMinutes(-1);
        var command = new ProcessSyncBatchCommand
        {
            DeviceId = "dev-1",
            EngineerId = "eng-1",
            Items = new[]
            {
                new SyncBatchItem { OperationType = "CheckIn", Payload = "{}", CreatedOnDeviceUtc = duplicateTime },
                new SyncBatchItem { OperationType = "CheckIn", Payload = "{}", CreatedOnDeviceUtc = freshTime }
            }
        };

        var syncQueueRepository = new Mock<ISyncQueueRepository>();
        syncQueueRepository
            .Setup(r => r.ExistsDuplicateAsync(
                "dev-1",
                "eng-1",
                "CheckIn",
                It.IsAny<DateTime>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((string _, string _, string _, DateTime createdAt, CancellationToken _) => createdAt == duplicateTime);

        var processor = new Mock<ISyncQueueProcessor>();
        processor
            .Setup(p => p.ProcessAsync(It.IsAny<IReadOnlyList<SyncQueue>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SyncResultDto
            {
                Processed = 1,
                Conflicts = 0,
                Failed = 0
            });

        var settings = new Mock<ISystemSettingsService>();
        settings
            .Setup(s => s.GetAsync("Sync:MaxBatchSize", 50, It.IsAny<CancellationToken>()))
            .ReturnsAsync(50);

        var unitOfWork = new Mock<IUnitOfWork>();

        var handler = new ProcessSyncBatchCommandHandler(
            syncQueueRepository.Object,
            processor.Object,
            settings.Object,
            unitOfWork.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Skipped.Should().Be(1);
        result.Value.Processed.Should().Be(1);
        processor.Verify(p => p.ProcessAsync(It.Is<IReadOnlyList<SyncQueue>>(x => x.Count == 1), It.IsAny<CancellationToken>()), Times.Once);
    }
}
