using System.Diagnostics;
using ClosedXML.Excel;
using FluentAssertions;
using MediatR;
using Moq;
using TowerOps.Application.Commands.Sites.CreateSite;
using TowerOps.Application.Commands.Sites.ImportSiteData;
using TowerOps.Application.Commands.Sync.ProcessSyncBatch;
using TowerOps.Application.Common;
using TowerOps.Application.Common.Interfaces;
using TowerOps.Application.DTOs.Portal;
using TowerOps.Application.DTOs.Sites;
using TowerOps.Application.DTOs.Sync;
using TowerOps.Application.Queries.Portal.GetPortalSites;
using TowerOps.Application.Services;
using TowerOps.Domain.Entities.Offices;
using TowerOps.Domain.Entities.Users;
using TowerOps.Domain.Enums;
using TowerOps.Domain.Interfaces.Repositories;
using TowerOps.Domain.ValueObjects;
using Xunit;

namespace TowerOps.Application.Tests.Performance;

public class PerformanceBaselineSmokeTests
{
    [Fact]
    public async Task Baseline_PortalSitesQuery_ShouldCompleteWithinThreshold()
    {
        var user = User.Create("Portal User", "portal@test.com", "01000000001", UserRole.Supervisor, Guid.NewGuid());
        user.EnableClientPortalAccess("ORANGE");

        var currentUser = new Mock<ICurrentUserService>();
        currentUser.SetupGet(x => x.UserId).Returns(user.Id);

        var users = new Mock<IUserRepository>();
        users.Setup(x => x.GetByIdAsNoTrackingAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var readRepo = new Mock<IPortalReadRepository>();
        readRepo.Setup(x => x.GetSitesAsync("ORANGE", null, 1, 200, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Enumerable.Range(1, 200).Select(i => new PortalSiteDto
            {
                SiteCode = $"S-{i:0000}",
                Name = $"Site {i}",
                Region = "Region",
                Status = SiteStatus.OnAir
            }).ToList());

        var sut = new GetPortalSitesQueryHandler(currentUser.Object, users.Object, readRepo.Object);

        var sw = Stopwatch.StartNew();
        for (var i = 0; i < 100; i++)
        {
            var result = await sut.Handle(new GetPortalSitesQuery
            {
                PageNumber = 1,
                PageSize = 200
            }, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(200);
        }
        sw.Stop();

        sw.ElapsedMilliseconds.Should().BeLessThan(3000);
    }

    [Fact]
    public async Task Baseline_ImportSiteData_500Rows_ShouldCompleteWithinThreshold()
    {
        var officeRepo = new Mock<IOfficeRepository>();
        officeRepo.Setup(x => x.GetByCodeAsNoTrackingAsync("CAI", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Office.Create("CAI", "Cairo Office", "Cairo", Address.Create("Street", "Cairo", "Cairo")));

        var sender = new Mock<ISender>();
        sender.Setup(x => x.Send(It.IsAny<CreateSiteCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(new SiteDetailDto { Id = Guid.NewGuid() }));

        var settings = BuildImportSettings(skipInvalidRows: true, maxRows: 5000);
        var sut = new ImportSiteDataCommandHandler(officeRepo.Object, sender.Object, settings.Object);

        var command = new ImportSiteDataCommand { FileContent = BuildWorkbookBytes(500) };

        var sw = Stopwatch.StartNew();
        var result = await sut.Handle(command, CancellationToken.None);
        sw.Stop();

        result.IsSuccess.Should().BeTrue();
        result.Value!.ImportedCount.Should().Be(500);
        sw.ElapsedMilliseconds.Should().BeLessThan(8000);
    }

    [Fact]
    public async Task Baseline_SyncBatch50Items_ShouldCompleteWithinThreshold()
    {
        var command = new ProcessSyncBatchCommand
        {
            DeviceId = "dev-baseline",
            EngineerId = "eng-baseline",
            Items = Enumerable.Range(1, 50)
                .Select(i => new SyncBatchItem
                {
                    OperationType = "CheckIn",
                    Payload = "{}",
                    CreatedOnDeviceUtc = DateTime.UtcNow.AddSeconds(-i)
                })
                .ToList()
        };

        var syncQueueRepository = new Mock<ISyncQueueRepository>();
        syncQueueRepository
            .Setup(r => r.ExistsDuplicateAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<DateTime>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        syncQueueRepository
            .Setup(r => r.AddAsync(It.IsAny<Domain.Entities.Sync.SyncQueue>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var processor = new Mock<ISyncQueueProcessor>();
        processor
            .Setup(p => p.ProcessAsync(It.IsAny<IReadOnlyList<Domain.Entities.Sync.SyncQueue>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SyncResultDto
            {
                Processed = 50,
                Conflicts = 0,
                Failed = 0,
                Skipped = 0
            });

        var settings = new Mock<ISystemSettingsService>();
        settings
            .Setup(s => s.GetAsync("Sync:MaxBatchSize", 50, It.IsAny<CancellationToken>()))
            .ReturnsAsync(50);

        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new ProcessSyncBatchCommandHandler(
            syncQueueRepository.Object,
            processor.Object,
            settings.Object,
            unitOfWork.Object);

        var sw = Stopwatch.StartNew();
        var result = await handler.Handle(command, CancellationToken.None);
        sw.Stop();

        result.IsSuccess.Should().BeTrue();
        result.Value!.Processed.Should().Be(50);
        sw.ElapsedMilliseconds.Should().BeLessThan(3000);
    }

    private static Mock<ISystemSettingsService> BuildImportSettings(bool skipInvalidRows, int maxRows)
    {
        var settings = new Mock<ISystemSettingsService>();
        settings.Setup(x => x.GetAsync("Import:SkipInvalidRows", true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(skipInvalidRows);
        settings.Setup(x => x.GetAsync("Import:MaxRows", 5000, It.IsAny<CancellationToken>()))
            .ReturnsAsync(maxRows);
        settings.Setup(x => x.GetAsync("Import:MaxFileSizeBytes", 10 * 1024 * 1024, It.IsAny<CancellationToken>()))
            .ReturnsAsync(10 * 1024 * 1024);
        settings.Setup(x => x.GetAsync("Import:DefaultDateFormat", "dd/MM/yyyy", It.IsAny<CancellationToken>()))
            .ReturnsAsync("dd/MM/yyyy");
        return settings;
    }

    private static byte[] BuildWorkbookBytes(int rowCount)
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.AddWorksheet("Sites");
        ws.Cell(1, 1).Value = "SiteCode";
        ws.Cell(1, 2).Value = "SiteName";
        ws.Cell(1, 3).Value = "OfficeCode";
        ws.Cell(1, 4).Value = "Region";
        ws.Cell(1, 5).Value = "SubRegion";
        ws.Cell(1, 6).Value = "SiteType";
        ws.Cell(1, 7).Value = "Latitude";
        ws.Cell(1, 8).Value = "Longitude";

        for (var i = 0; i < rowCount; i++)
        {
            var row = i + 2;
            ws.Cell(row, 1).Value = $"CAI{i + 1:000}";
            ws.Cell(row, 2).Value = $"Site {i + 1}";
            ws.Cell(row, 3).Value = "CAI";
            ws.Cell(row, 4).Value = "R1";
            ws.Cell(row, 5).Value = "SR1";
            ws.Cell(row, 6).Value = "Macro";
            ws.Cell(row, 7).Value = "30.1";
            ws.Cell(row, 8).Value = "31.2";
        }

        using var ms = new MemoryStream();
        workbook.SaveAs(ms);
        return ms.ToArray();
    }
}
