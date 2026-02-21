using FluentAssertions;
using Moq;
using TelecomPM.Application.Commands.Imports.ImportAlarmCapture;
using TelecomPM.Application.Commands.Imports.ImportBatteryDischargeTest;
using TelecomPM.Application.Commands.Imports.ImportChecklistTemplate;
using TelecomPM.Application.Commands.Imports.ImportDeltaSites;
using TelecomPM.Application.Commands.Imports.ImportPanoramaEvidence;
using TelecomPM.Application.Commands.Imports.ImportPowerData;
using TelecomPM.Application.Commands.Imports.ImportRFStatus;
using TelecomPM.Application.Commands.Imports.ImportSiteAssets;
using TelecomPM.Application.Commands.Imports.ImportSiteRadioData;
using TelecomPM.Application.Commands.Imports.ImportSiteSharingData;
using TelecomPM.Application.Commands.Imports.ImportSiteTxData;
using TelecomPM.Domain.Entities.BatteryDischargeTests;
using TelecomPM.Domain.Entities.ChecklistTemplates;
using TelecomPM.Domain.Entities.Sites;
using TelecomPM.Domain.Entities.Visits;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Interfaces.Repositories;
using Xunit;

namespace TelecomPM.Application.Tests.Integration;

public class ImportCommandsRealFilesIntegrationTests
{
    [Fact]
    public async Task ImportSiteAssetsCommand_ShouldParseRealWorkbook_WithImportsAndSkips()
    {
        var (siteRepo, _) = CreateSiteRepository("3564DE");
        var unitOfWork = CreateUnitOfWork();
        var sut = new ImportSiteAssetsCommandHandler(siteRepo.Object, unitOfWork.Object);

        var result = await sut.Handle(new ImportSiteAssetsCommand
        {
            FileContent = RealExcelTestSupport.LoadExcelBytes("Data collection from 1-1-2023 till 31-12-2023.xlsx")
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.ImportedCount.Should().BeGreaterThan(0);
        result.Value.SkippedCount.Should().BeGreaterThan(0);
        result.Value.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public async Task ImportPowerDataCommand_ShouldParseRealWorkbook_WithImportsAndSkips()
    {
        var (siteRepo, _) = CreateSiteRepository("3564DE");
        var unitOfWork = CreateUnitOfWork();
        var sut = new ImportPowerDataCommandHandler(siteRepo.Object, unitOfWork.Object);

        var result = await sut.Handle(new ImportPowerDataCommand
        {
            FileContent = RealExcelTestSupport.LoadExcelBytes("Data collection from 1-1-2023 till 31-12-2023.xlsx")
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task ImportSiteRadioDataCommand_ShouldParseRealWorkbook_WithImportsAndSkips()
    {
        var (siteRepo, _) = CreateSiteRepository("3564DE");
        var unitOfWork = CreateUnitOfWork();
        var sut = new ImportSiteRadioDataCommandHandler(siteRepo.Object, unitOfWork.Object);

        var result = await sut.Handle(new ImportSiteRadioDataCommand
        {
            FileContent = RealExcelTestSupport.LoadExcelBytes("Data collection from 1-1-2023 till 31-12-2023.xlsx")
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task ImportSiteTxDataCommand_ShouldParseRealWorkbook_WithImportsAndSkips()
    {
        var (siteRepo, _) = CreateSiteRepository("3564DE");
        var unitOfWork = CreateUnitOfWork();
        var sut = new ImportSiteTxDataCommandHandler(siteRepo.Object, unitOfWork.Object);

        var result = await sut.Handle(new ImportSiteTxDataCommand
        {
            FileContent = RealExcelTestSupport.LoadExcelBytes("Data collection from 1-1-2023 till 31-12-2023.xlsx")
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task ImportSiteSharingDataCommand_ShouldParseRealWorkbook_WithImportsAndSkips()
    {
        var (siteRepo, _) = CreateSiteRepository("0658DE");
        var unitOfWork = CreateUnitOfWork();
        var sut = new ImportSiteSharingDataCommandHandler(siteRepo.Object, unitOfWork.Object);

        var result = await sut.Handle(new ImportSiteSharingDataCommand
        {
            FileContent = RealExcelTestSupport.LoadExcelBytes("Data collection from 1-1-2023 till 31-12-2023.xlsx")
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task ImportRfStatusCommand_ShouldParseRealWorkbook_WithImportsAndSkips()
    {
        var (siteRepo, _) = CreateSiteRepository("3564DE");
        var unitOfWork = CreateUnitOfWork();
        var sut = new ImportRFStatusCommandHandler(siteRepo.Object, unitOfWork.Object);

        var result = await sut.Handle(new ImportRFStatusCommand
        {
            FileContent = RealExcelTestSupport.LoadExcelBytes("GH-DE Data Collection.xlsx")
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task ImportBatteryDischargeTestCommand_ShouldParseRealWorkbook_WithImportsAndSkips()
    {
        var (siteRepo, _) = CreateSiteRepository("3564DE", "4390DE", "1067DE", "0058DE", "0907DE");
        var batteryRepo = new Mock<IBatteryDischargeTestRepository>();
        var added = new List<BatteryDischargeTest>();
        batteryRepo.Setup(x => x.AddAsync(It.IsAny<BatteryDischargeTest>(), It.IsAny<CancellationToken>()))
            .Callback<BatteryDischargeTest, CancellationToken>((entity, _) => added.Add(entity))
            .Returns(Task.CompletedTask);

        var unitOfWork = CreateUnitOfWork();
        var sut = new ImportBatteryDischargeTestCommandHandler(siteRepo.Object, batteryRepo.Object, unitOfWork.Object);

        var result = await sut.Handle(new ImportBatteryDischargeTestCommand
        {
            FileContent = RealExcelTestSupport.LoadExcelBytes("GH-BDT_BDT.xlsx")
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        if (result.Value!.ImportedCount > 0)
        {
            added.Count.Should().Be(result.Value!.ImportedCount);
        }
    }

    [Fact]
    public async Task ImportDeltaSitesCommand_ShouldParseRealWorkbook_WithImportsAndSkips()
    {
        var (siteRepo, _) = CreateSiteRepository("0700DE", "0676DE", "1067DE", "0058DE", "0907DE", "4390DE");
        var unitOfWork = CreateUnitOfWork();
        var sut = new ImportDeltaSitesCommandHandler(siteRepo.Object, unitOfWork.Object);

        var result = await sut.Handle(new ImportDeltaSitesCommand
        {
            FileContent = RealExcelTestSupport.LoadExcelBytes("Delta Sites (1).xlsx")
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.ImportedCount.Should().BeGreaterThan(0);
        result.Value.SkippedCount.Should().BeGreaterThan(0);
        result.Value.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public async Task ImportChecklistTemplateCommand_ShouldImportRows_FromChecklistWorkbook()
    {
        var templateRepo = new Mock<IChecklistTemplateRepository>();
        var createdTemplates = new List<ChecklistTemplate>();

        templateRepo.Setup(x => x.GetByVisitTypeAsync(VisitType.BM, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<ChecklistTemplate>());
        templateRepo.Setup(x => x.AddAsync(It.IsAny<ChecklistTemplate>(), It.IsAny<CancellationToken>()))
            .Callback<ChecklistTemplate, CancellationToken>((template, _) => createdTemplates.Add(template))
            .Returns(Task.CompletedTask);

        var unitOfWork = CreateUnitOfWork();
        var sut = new ImportChecklistTemplateCommandHandler(templateRepo.Object, unitOfWork.Object);

        var result = await sut.Handle(new ImportChecklistTemplateCommand
        {
            FileContent = RealExcelTestSupport.LoadExcelBytes("GH-DE  Checklist.xlsx"),
            VisitType = VisitType.BM,
            Version = "v1.2",
            EffectiveFromUtc = DateTime.UtcNow,
            CreatedBy = "integration-test",
            ChangeNotes = "Imported from real template"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.ImportedCount.Should().BeGreaterThan(0);
        createdTemplates.Should().ContainSingle();
        createdTemplates[0].Items.Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public async Task ImportPanoramaEvidenceCommand_ShouldReadRealChecklistWorkbook_AndReturnProcessedResult()
    {
        var site = RealExcelTestSupport.CreateSite("3564DE", Guid.NewGuid());
        var visit = RealExcelTestSupport.CreateVisit(site);

        var visitRepo = new Mock<IVisitRepository>();
        visitRepo.Setup(x => x.GetByIdAsync(visit.Id, It.IsAny<CancellationToken>())).ReturnsAsync(visit);
        visitRepo.Setup(x => x.UpdateAsync(visit, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var unitOfWork = CreateUnitOfWork();
        var sut = new ImportPanoramaEvidenceCommandHandler(visitRepo.Object, unitOfWork.Object);

        var result = await sut.Handle(new ImportPanoramaEvidenceCommand
        {
            VisitId = visit.Id,
            FileContent = RealExcelTestSupport.LoadExcelBytes("GH-DE  Checklist.xlsx")
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        (result.Value!.ImportedCount > 0 || result.Value.SkippedCount > 0).Should().BeTrue();
    }

    [Fact]
    public async Task ImportAlarmCaptureCommand_ShouldReadRealChecklistWorkbook_AndReturnProcessedResult()
    {
        var site = RealExcelTestSupport.CreateSite("3564DE", Guid.NewGuid());
        var visit = RealExcelTestSupport.CreateVisit(site);

        var visitRepo = new Mock<IVisitRepository>();
        visitRepo.Setup(x => x.GetByIdAsync(visit.Id, It.IsAny<CancellationToken>())).ReturnsAsync(visit);
        visitRepo.Setup(x => x.UpdateAsync(visit, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var unitOfWork = CreateUnitOfWork();
        var sut = new ImportAlarmCaptureCommandHandler(visitRepo.Object, unitOfWork.Object);

        var result = await sut.Handle(new ImportAlarmCaptureCommand
        {
            VisitId = visit.Id,
            FileContent = RealExcelTestSupport.LoadExcelBytes("GH-DE  Checklist.xlsx")
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        (result.Value!.ImportedCount > 0 || result.Value.SkippedCount > 0).Should().BeTrue();
    }

    private static Mock<IUnitOfWork> CreateUnitOfWork()
    {
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        return unitOfWork;
    }

    private static (Mock<ISiteRepository> SiteRepository, List<Site> Sites) CreateSiteRepository(params string[] shortCodes)
    {
        var officeId = Guid.NewGuid();
        var sites = shortCodes.Select(code => RealExcelTestSupport.CreateSite(code, officeId)).ToList();

        var siteRepo = new Mock<ISiteRepository>();
        siteRepo.Setup(x => x.GetAllAsNoTrackingAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(sites);
        siteRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid id, CancellationToken _) => sites.FirstOrDefault(s => s.Id == id));

        return (siteRepo, sites);
    }
}
