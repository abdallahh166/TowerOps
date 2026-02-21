using ClosedXML.Excel;
using FluentAssertions;
using MediatR;
using Moq;
using TelecomPM.Application.Common;
using TelecomPM.Application.Commands.Reports.ExportBDT;
using TelecomPM.Application.Commands.Reports.ExportChecklist;
using TelecomPM.Application.Commands.Reports.ExportDataCollection;
using TelecomPM.Application.Commands.Reports.ExportScorecard;
using TelecomPM.Application.Commands.Reports.GenerateContractorScorecard;
using TelecomPM.Domain.Entities.BatteryDischargeTests;
using TelecomPM.Domain.Entities.ChecklistTemplates;
using TelecomPM.Domain.Entities.Offices;
using TelecomPM.Domain.Entities.Sites;
using TelecomPM.Domain.Entities.Visits;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Interfaces.Repositories;
using TelecomPM.Domain.ValueObjects;
using Xunit;

namespace TelecomPM.Application.Tests.Integration;

public class ExportCommandsRealFilesIntegrationTests
{
    [Fact]
    public async Task ExportChecklistCommand_ShouldGenerateWorkbook_WithTemplateSheetCoverage()
    {
        var site = RealExcelTestSupport.CreateSite("3564DE", Guid.NewGuid());
        var visit = RealExcelTestSupport.CreateVisit(site);
        visit.AddReading(VisitReading.Create(visit.Id, "Rectifier DC Voltage", "Power", 53m, "V"));
        visit.AddPhoto(VisitPhoto.Create(visit.Id, PhotoType.Before, PhotoCategory.ShelterInside, "Before", "before.jpg", "/before.jpg"));
        visit.AddPhoto(VisitPhoto.Create(visit.Id, PhotoType.After, PhotoCategory.Tower, "After", "after.jpg", "/after.jpg"));

        var template = ChecklistTemplate.Create(VisitType.BM, "v1.0", DateTime.UtcNow, "seed");
        template.AddItem("Power", "Rectifier Visual Check", null, true, 1);
        template.Activate("approver");

        var visitRepo = new Mock<IVisitRepository>();
        visitRepo.Setup(x => x.GetAllAsNoTrackingAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Visit> { visit });
        visitRepo.Setup(x => x.GetByIdAsNoTrackingAsync(visit.Id, It.IsAny<CancellationToken>())).ReturnsAsync(visit);

        var templateRepo = new Mock<IChecklistTemplateRepository>();
        templateRepo.Setup(x => x.GetByVisitTypeAsync(VisitType.BM, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ChecklistTemplate> { template });

        var sut = new ExportChecklistCommandHandler(visitRepo.Object, templateRepo.Object);
        var result = await sut.Handle(new ExportChecklistCommand { VisitType = VisitType.BM }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();

        using var exported = new XLWorkbook(new MemoryStream(result.Value!));
        using var source = RealExcelTestSupport.OpenWorkbook("GH-DE  Checklist.xlsx");

        var required = new[]
        {
            "site's reading",
            "Common checklist",
            "Panorama",
            "Tower Panorama",
            "Before & after",
            "Pending Res.",
            "unused assets",
            "alarms capture",
            "Audit matrix SQI"
        };

        var sourceNames = source.Worksheets.Select(w => w.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var exportedNames = exported.Worksheets.Select(w => w.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);

        required.Should().OnlyContain(name => sourceNames.Contains(name));
        required.Should().OnlyContain(name => exportedNames.Contains(name));
        exported.Worksheet("site's reading").Row(1).CellsUsed().Should().NotBeEmpty();
        exported.Worksheet("Common checklist").Row(1).CellsUsed().Should().NotBeEmpty();
    }

    [Fact]
    public async Task ExportBdtCommand_ShouldGenerateWorkbook_WithTemplateSheetCoverage()
    {
        var bdt = BatteryDischargeTest.Create(Guid.NewGuid(), "3564DE", DateTime.UtcNow);
        bdt.UpdateDetails(
            network: "NSN",
            siteCategory: "Shelter",
            powerSource: "EC",
            nodalDegree: 1,
            startVoltage: 52m,
            startAmperage: 10m,
            endVoltage: 47m,
            endAmperage: 5m,
            dischargeTimeMinutes: 120,
            reasonForStop: "BATTERIES ARE DISCHARGED",
            reasonForRepeatedBdt: "Cycle",
            week: "W01");

        var repo = new Mock<IBatteryDischargeTestRepository>();
        repo.Setup(x => x.GetAllAsNoTrackingAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<BatteryDischargeTest> { bdt });

        var sut = new ExportBDTCommandHandler(repo.Object);
        var result = await sut.Handle(new ExportBDTCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();

        using var exported = new XLWorkbook(new MemoryStream(result.Value!));
        using var source = RealExcelTestSupport.OpenWorkbook("GH-BDT_BDT.xlsx");

        var sourceNames = source.Worksheets.Select(w => w.Name.Trim()).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var exportedNames = exported.Worksheets.Select(w => w.Name.Trim()).ToHashSet(StringComparer.OrdinalIgnoreCase);

        var required = new[] { "Summary", "BDT sheet", "Power Alarm", "Config" };
        required.Should().OnlyContain(name => sourceNames.Contains(name));
        required.Should().OnlyContain(name => exportedNames.Contains(name));
        exported.Worksheet("Summary").Cell(1, 1).GetString().Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task ExportDataCollectionCommand_ShouldGenerateWorkbook_WithTemplateSheetCoverage()
    {
        var office = Office.Create("CAI", "Cairo Office", "Cairo", Address.Create("Street", "Cairo", "Cairo"));
        var site = RealExcelTestSupport.CreateSite("3564DE", office.Id);

        var officeRepo = new Mock<IOfficeRepository>();
        officeRepo.Setup(x => x.GetByCodeAsNoTrackingAsync("CAI", It.IsAny<CancellationToken>()))
            .ReturnsAsync(office);

        var siteRepo = new Mock<ISiteRepository>();
        siteRepo.Setup(x => x.GetByOfficeIdAsNoTrackingAsync(office.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Site> { site });
        siteRepo.Setup(x => x.GetByIdAsNoTrackingAsync(site.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(site);

        var sut = new ExportDataCollectionCommandHandler(siteRepo.Object, officeRepo.Object);
        var result = await sut.Handle(new ExportDataCollectionCommand { OfficeCode = "CAI" }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();

        using var exported = new XLWorkbook(new MemoryStream(result.Value!));
        using var source = RealExcelTestSupport.OpenWorkbook("GH-DE Data Collection.xlsx");

        var required = new[]
        {
            "Site Assets Data Count",
            "Power Data",
            "Site Radio Data",
            "Site TX Data",
            "Site Sharing Data",
            "RF Status"
        };

        var sourceNames = source.Worksheets.Select(w => w.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var exportedNames = exported.Worksheets.Select(w => w.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);

        required.Should().OnlyContain(name => sourceNames.Contains(name));
        required.Should().OnlyContain(name => exportedNames.Contains(name));
        exported.Worksheet("Power Data").Row(1).CellsUsed().Should().NotBeEmpty();
    }

    [Fact]
    public async Task ExportScorecardCommand_ShouldReturnBytes_FromMediatRPipeline_WithRealWorkbookPayload()
    {
        var realPayload = RealExcelTestSupport.LoadExcelBytes("Data collection from 1-1-2023 till 31-12-2023.xlsx");
        var sender = new Mock<ISender>();
        sender.Setup(x => x.Send(It.IsAny<GenerateContractorScorecardCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(realPayload));

        var sut = new ExportScorecardCommandHandler(sender.Object);
        var result = await sut.Handle(new ExportScorecardCommand
        {
            OfficeCode = "CAI",
            Month = 2,
            Year = 2026
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();

        using var workbook = new XLWorkbook(new MemoryStream(result.Value!));
        workbook.Worksheets.Count.Should().BeGreaterThan(0);
    }
}
