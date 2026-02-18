using System.Text;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TelecomPM.Application.Common.Interfaces;
using TelecomPM.Domain.Entities.Materials;
using TelecomPM.Domain.Entities.Sites;
using TelecomPM.Domain.Entities.Visits;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Interfaces.Repositories;
using TelecomPM.Domain.ValueObjects;
using TelecomPM.Infrastructure.Services;
using Xunit;

namespace TelecomPM.Infrastructure.Tests.Services;

public class ReportGenerationServiceTests
{
    [Fact]
    public async Task GenerateVisitReportAsync_WithExcel_ShouldUseExcelExportService()
    {
        var visit = CreateVisit();
        var site = CreateSite();

        var visitRepo = new Mock<IVisitRepository>();
        visitRepo.Setup(x => x.GetByIdAsync(visit.Id, It.IsAny<CancellationToken>())).ReturnsAsync(visit);

        var siteRepo = new Mock<ISiteRepository>();
        siteRepo.Setup(x => x.GetByIdAsync(visit.SiteId, It.IsAny<CancellationToken>())).ReturnsAsync(site);

        var materialRepo = new Mock<IMaterialRepository>();
        var excel = new Mock<IExcelExportService>();
        excel.Setup(x => x.ExportVisitToExcelAsync(visit.Id, It.IsAny<CancellationToken>())).ReturnsAsync(new byte[] { 1, 2, 3 });

        var sut = new ReportGenerationService(
            visitRepo.Object,
            siteRepo.Object,
            materialRepo.Object,
            excel.Object,
            Mock.Of<ILogger<ReportGenerationService>>());

        var bytes = await sut.GenerateVisitReportAsync(visit.Id, ReportFormat.Excel);

        bytes.Should().Equal(new byte[] { 1, 2, 3 });
    }

    [Fact]
    public async Task GenerateMaterialConsumptionReportAsync_ShouldReturnCsv()
    {
        var material = Material.Create(
            "MAT-001",
            "Battery",
            "Battery",
            MaterialCategory.Power,
            Guid.NewGuid(),
            MaterialQuantity.Create(10, MaterialUnit.Pieces),
            MaterialQuantity.Create(2, MaterialUnit.Pieces),
            Money.Create(100, "EGP"));

        material.DeductStock(MaterialQuantity.Create(1, MaterialUnit.Pieces));

        var visitRepo = new Mock<IVisitRepository>();
        var siteRepo = new Mock<ISiteRepository>();
        var materialRepo = new Mock<IMaterialRepository>();
        materialRepo.Setup(x => x.GetByOfficeIdAsNoTrackingAsync(material.OfficeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Material> { material });

        var sut = new ReportGenerationService(
            visitRepo.Object,
            siteRepo.Object,
            materialRepo.Object,
            Mock.Of<IExcelExportService>(),
            Mock.Of<ILogger<ReportGenerationService>>());

        var result = await sut.GenerateMaterialConsumptionReportAsync(material.OfficeId, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1));

        var csv = Encoding.UTF8.GetString(result);
        csv.Should().Contain("MaterialCode,MaterialName,TransactionDateUtc");
        csv.Should().Contain("MAT-001");
        csv.Should().Contain("Usage");
    }

    [Fact]
    public async Task GenerateEngineerPerformanceReportAsync_ShouldReturnAggregatedCsv()
    {
        var visit = CreateVisit();
        var visitRepo = new Mock<IVisitRepository>();
        visitRepo.Setup(x => x.GetByEngineerIdAsNoTrackingAsync(visit.EngineerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Visit> { visit });

        var sut = new ReportGenerationService(
            visitRepo.Object,
            Mock.Of<ISiteRepository>(),
            Mock.Of<IMaterialRepository>(),
            Mock.Of<IExcelExportService>(),
            Mock.Of<ILogger<ReportGenerationService>>());

        var result = await sut.GenerateEngineerPerformanceReportAsync(visit.EngineerId, DateTime.UtcNow.AddDays(-7), DateTime.UtcNow.AddDays(1));

        var csv = Encoding.UTF8.GetString(result);
        csv.Should().Contain("VisitsScheduled");
        csv.Should().Contain(visit.EngineerId.ToString());
    }

    private static Visit CreateVisit()
    {
        return Visit.Create(
            "V0001",
            Guid.NewGuid(),
            "SITE-001",
            "Site 1",
            Guid.NewGuid(),
            "Eng 1",
            DateTime.UtcNow,
            VisitType.PreventiveMaintenance);
    }

    private static Site CreateSite()
    {
        return Site.Create(
            "SITE-001",
            "Site 1",
            "OMC-1",
            Guid.NewGuid(),
            "Cairo",
            "Nasr City",
            Coordinates.Create(30.01, 31.01),
            Address.Create("Street 1", "Cairo", "Cairo", "Egypt"),
            SiteType.Macro);
    }
}
