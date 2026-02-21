using System.Text;
using FluentAssertions;
using Moq;
using TelecomPM.Application.Common;
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
using TelecomPM.Application.DTOs.Sites;
using TelecomPM.Domain.Entities.BatteryDischargeTests;
using TelecomPM.Domain.Entities.ChecklistTemplates;
using TelecomPM.Domain.Entities.Sites;
using TelecomPM.Domain.Entities.Visits;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Interfaces.Repositories;
using Xunit;

namespace TelecomPM.Application.Tests.Integration;

public class Sprint12DryRunReconciliationTests
{
    [Fact]
    public async Task DryRun_ShouldGenerateReconciliationMarkdownReport()
    {
        var officeId = Guid.NewGuid();
        var sites = new List<Site>
        {
            RealExcelTestSupport.CreateSite("3564DE", officeId),
            RealExcelTestSupport.CreateSite("4390DE", officeId),
            RealExcelTestSupport.CreateSite("0658DE", officeId),
            RealExcelTestSupport.CreateSite("0700DE", officeId),
            RealExcelTestSupport.CreateSite("0676DE", officeId),
            RealExcelTestSupport.CreateSite("1067DE", officeId),
            RealExcelTestSupport.CreateSite("0058DE", officeId),
            RealExcelTestSupport.CreateSite("0907DE", officeId)
        };

        var preSnapshot = CaptureSnapshot(sites);
        var visit = RealExcelTestSupport.CreateVisit(sites[0]);

        var siteRepo = new Mock<ISiteRepository>();
        siteRepo.Setup(x => x.GetAllAsNoTrackingAsync(It.IsAny<CancellationToken>())).ReturnsAsync(sites);
        siteRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid id, CancellationToken _) => sites.FirstOrDefault(s => s.Id == id));

        var visitRepo = new Mock<IVisitRepository>();
        visitRepo.Setup(x => x.GetByIdAsync(visit.Id, It.IsAny<CancellationToken>())).ReturnsAsync(visit);
        visitRepo.Setup(x => x.UpdateAsync(visit, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var templateRepo = new Mock<IChecklistTemplateRepository>();
        var addedTemplates = new List<ChecklistTemplate>();
        templateRepo.Setup(x => x.GetByVisitTypeAsync(VisitType.BM, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<ChecklistTemplate>());
        templateRepo.Setup(x => x.AddAsync(It.IsAny<ChecklistTemplate>(), It.IsAny<CancellationToken>()))
            .Callback<ChecklistTemplate, CancellationToken>((entity, _) => addedTemplates.Add(entity))
            .Returns(Task.CompletedTask);

        var bdtRepo = new Mock<IBatteryDischargeTestRepository>();
        var addedBdts = new List<BatteryDischargeTest>();
        bdtRepo.Setup(x => x.AddAsync(It.IsAny<BatteryDischargeTest>(), It.IsAny<CancellationToken>()))
            .Callback<BatteryDischargeTest, CancellationToken>((entity, _) => addedBdts.Add(entity))
            .Returns(Task.CompletedTask);

        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var runs = new List<RunResult>();

        runs.Add(await Run("ImportSiteAssetsCommand", "Data collection from 1-1-2023 till 31-12-2023.xlsx",
            () => new ImportSiteAssetsCommandHandler(siteRepo.Object, unitOfWork.Object).Handle(
                new ImportSiteAssetsCommand { FileContent = RealExcelTestSupport.LoadExcelBytes("Data collection from 1-1-2023 till 31-12-2023.xlsx") },
                CancellationToken.None)));

        runs.Add(await Run("ImportPowerDataCommand", "Data collection from 1-1-2023 till 31-12-2023.xlsx",
            () => new ImportPowerDataCommandHandler(siteRepo.Object, unitOfWork.Object).Handle(
                new ImportPowerDataCommand { FileContent = RealExcelTestSupport.LoadExcelBytes("Data collection from 1-1-2023 till 31-12-2023.xlsx") },
                CancellationToken.None)));

        runs.Add(await Run("ImportSiteRadioDataCommand", "Data collection from 1-1-2023 till 31-12-2023.xlsx",
            () => new ImportSiteRadioDataCommandHandler(siteRepo.Object, unitOfWork.Object).Handle(
                new ImportSiteRadioDataCommand { FileContent = RealExcelTestSupport.LoadExcelBytes("Data collection from 1-1-2023 till 31-12-2023.xlsx") },
                CancellationToken.None)));

        runs.Add(await Run("ImportSiteTxDataCommand", "Data collection from 1-1-2023 till 31-12-2023.xlsx",
            () => new ImportSiteTxDataCommandHandler(siteRepo.Object, unitOfWork.Object).Handle(
                new ImportSiteTxDataCommand { FileContent = RealExcelTestSupport.LoadExcelBytes("Data collection from 1-1-2023 till 31-12-2023.xlsx") },
                CancellationToken.None)));

        runs.Add(await Run("ImportSiteSharingDataCommand", "Data collection from 1-1-2023 till 31-12-2023.xlsx",
            () => new ImportSiteSharingDataCommandHandler(siteRepo.Object, unitOfWork.Object).Handle(
                new ImportSiteSharingDataCommand { FileContent = RealExcelTestSupport.LoadExcelBytes("Data collection from 1-1-2023 till 31-12-2023.xlsx") },
                CancellationToken.None)));

        runs.Add(await Run("ImportRFStatusCommand", "GH-DE Data Collection.xlsx",
            () => new ImportRFStatusCommandHandler(siteRepo.Object, unitOfWork.Object).Handle(
                new ImportRFStatusCommand { FileContent = RealExcelTestSupport.LoadExcelBytes("GH-DE Data Collection.xlsx") },
                CancellationToken.None)));

        runs.Add(await Run("ImportBatteryDischargeTestCommand", "GH-BDT_BDT.xlsx",
            () => new ImportBatteryDischargeTestCommandHandler(siteRepo.Object, bdtRepo.Object, unitOfWork.Object).Handle(
                new ImportBatteryDischargeTestCommand { FileContent = RealExcelTestSupport.LoadExcelBytes("GH-BDT_BDT.xlsx") },
                CancellationToken.None)));

        runs.Add(await Run("ImportDeltaSitesCommand", "Delta Sites (1).xlsx",
            () => new ImportDeltaSitesCommandHandler(siteRepo.Object, unitOfWork.Object).Handle(
                new ImportDeltaSitesCommand { FileContent = RealExcelTestSupport.LoadExcelBytes("Delta Sites (1).xlsx") },
                CancellationToken.None)));

        runs.Add(await Run("ImportChecklistTemplateCommand", "GH-DE  Checklist.xlsx",
            () => new ImportChecklistTemplateCommandHandler(templateRepo.Object, unitOfWork.Object).Handle(
                new ImportChecklistTemplateCommand
                {
                    FileContent = RealExcelTestSupport.LoadExcelBytes("GH-DE  Checklist.xlsx"),
                    VisitType = VisitType.BM,
                    Version = "v1.2",
                    EffectiveFromUtc = DateTime.UtcNow,
                    CreatedBy = "dry-run",
                    ChangeNotes = "Sprint 12 dry-run"
                },
                CancellationToken.None)));

        runs.Add(await Run("ImportPanoramaEvidenceCommand", "GH-DE  Checklist.xlsx",
            () => new ImportPanoramaEvidenceCommandHandler(visitRepo.Object, unitOfWork.Object).Handle(
                new ImportPanoramaEvidenceCommand
                {
                    VisitId = visit.Id,
                    FileContent = RealExcelTestSupport.LoadExcelBytes("GH-DE  Checklist.xlsx")
                },
                CancellationToken.None)));

        runs.Add(await Run("ImportAlarmCaptureCommand", "GH-DE  Checklist.xlsx",
            () => new ImportAlarmCaptureCommandHandler(visitRepo.Object, unitOfWork.Object).Handle(
                new ImportAlarmCaptureCommand
                {
                    VisitId = visit.Id,
                    FileContent = RealExcelTestSupport.LoadExcelBytes("GH-DE  Checklist.xlsx")
                },
                CancellationToken.None)));

        var postSnapshot = CaptureSnapshot(sites);
        var report = BuildReport(runs, preSnapshot, postSnapshot, visit, addedTemplates, addedBdts);

        var reportPath = Path.Combine(RealExcelTestSupport.GetRepoRoot(), "docs", "phase-2", "10-sprint-12-dry-run-reconciliation.md");
        File.WriteAllText(reportPath, report, Encoding.UTF8);

        File.Exists(reportPath).Should().BeTrue();
        report.Should().Contain("Coverage Summary");
    }

    private static async Task<RunResult> Run(string commandName, string sourceFile, Func<Task<Result<ImportSiteDataResult>>> run)
    {
        var result = await run();
        if (result.IsFailure || result.Value is null)
        {
            return new RunResult(commandName, sourceFile, false, 0, 0, new List<string> { result.Error });
        }

        return new RunResult(
            commandName,
            sourceFile,
            true,
            result.Value.ImportedCount,
            result.Value.SkippedCount,
            result.Value.Errors.Take(10).ToList());
    }

    private static Dictionary<Guid, SiteSnapshot> CaptureSnapshot(IEnumerable<Site> sites)
    {
        return sites.ToDictionary(
            s => s.Id,
            s => new SiteSnapshot(
                HasPowerSystem: s.PowerSystem is not null,
                HasTransmission: s.Transmission is not null,
                MwLinks: s.Transmission?.MWLinks.Count ?? 0,
                HasRadio: s.RadioEquipment is not null,
                Sectors: s.RadioEquipment?.Sectors.Count ?? 0,
                HasSharing: s.SharingInfo is not null,
                SharingPositions: s.SharingInfo?.AntennaPositions.Count ?? 0,
                HasRfStatus: s.RFStatus is not null,
                OperationalZone: s.OperationalZone));
    }

    private static string BuildReport(
        List<RunResult> runs,
        Dictionary<Guid, SiteSnapshot> before,
        Dictionary<Guid, SiteSnapshot> after,
        Visit visit,
        List<ChecklistTemplate> templates,
        List<BatteryDischargeTest> bdts)
    {
        var sb = new StringBuilder();
        sb.AppendLine("# Sprint 12 Dry-Run Reconciliation Report");
        sb.AppendLine();
        sb.AppendLine($"Generated At UTC: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}");
        sb.AppendLine();
        sb.AppendLine("## Coverage Summary");
        sb.AppendLine("| Command | Source File | Success | Imported | Skipped |");
        sb.AppendLine("| --- | --- | --- | ---: | ---: |");

        foreach (var run in runs)
        {
            sb.AppendLine($"| {run.CommandName} | {run.SourceFile} | {(run.Success ? "Yes" : "No")} | {run.Imported} | {run.Skipped} |");
        }

        sb.AppendLine();
        sb.AppendLine("## Top Error Reasons");
        var groupedErrors = runs
            .SelectMany(r => r.Errors)
            .Where(e => !string.IsNullOrWhiteSpace(e))
            .GroupBy(e => e.Trim())
            .OrderByDescending(g => g.Count())
            .Take(15)
            .ToList();

        if (groupedErrors.Count == 0)
        {
            sb.AppendLine("- No errors captured.");
        }
        else
        {
            foreach (var group in groupedErrors)
            {
                sb.AppendLine($"- {group.Key} (x{group.Count()})");
            }
        }

        var sitesChanged = before.Keys.Count(id => !before[id].Equals(after[id]));
        var powerChanged = before.Keys.Count(id => !before[id].HasPowerSystem && after[id].HasPowerSystem);
        var transmissionChanged = before.Keys.Count(id => !before[id].HasTransmission && after[id].HasTransmission);
        var radioChanged = before.Keys.Count(id => !before[id].HasRadio && after[id].HasRadio);
        var sharingChanged = before.Keys.Count(id => !before[id].HasSharing && after[id].HasSharing);
        var rfChanged = before.Keys.Count(id => !before[id].HasRfStatus && after[id].HasRfStatus);
        var totalMwLinksAdded = before.Keys.Sum(id => Math.Max(0, after[id].MwLinks - before[id].MwLinks));
        var totalSectorsAdded = before.Keys.Sum(id => Math.Max(0, after[id].Sectors - before[id].Sectors));
        var totalSharingPositionsAdded = before.Keys.Sum(id => Math.Max(0, after[id].SharingPositions - before[id].SharingPositions));
        var ozUpdated = before.Keys.Count(id => !string.Equals(before[id].OperationalZone, after[id].OperationalZone, StringComparison.Ordinal));

        sb.AppendLine();
        sb.AppendLine("## Entity-Level Diff");
        sb.AppendLine($"- Sites changed: {sitesChanged}");
        sb.AppendLine($"- Sites with new power system state: {powerChanged}");
        sb.AppendLine($"- Sites with new transmission state: {transmissionChanged}");
        sb.AppendLine($"- Total MW links added: {totalMwLinksAdded}");
        sb.AppendLine($"- Sites with new radio state: {radioChanged}");
        sb.AppendLine($"- Total sector records added: {totalSectorsAdded}");
        sb.AppendLine($"- Sites with new sharing state: {sharingChanged}");
        sb.AppendLine($"- Total sharing antenna positions added: {totalSharingPositionsAdded}");
        sb.AppendLine($"- Sites with RF status changes: {rfChanged}");
        sb.AppendLine($"- Sites with operational zone changes: {ozUpdated}");
        sb.AppendLine($"- Checklist templates created: {templates.Count}");
        sb.AppendLine($"- Checklist items imported: {templates.Sum(t => t.Items.Count)}");
        sb.AppendLine($"- Battery discharge tests imported: {bdts.Count}");
        sb.AppendLine($"- Visit evidence photos after dry-run: {visit.Photos.Count}");

        return sb.ToString();
    }

    private sealed record RunResult(
        string CommandName,
        string SourceFile,
        bool Success,
        int Imported,
        int Skipped,
        List<string> Errors);

    private sealed record SiteSnapshot(
        bool HasPowerSystem,
        bool HasTransmission,
        int MwLinks,
        bool HasRadio,
        int Sectors,
        bool HasSharing,
        int SharingPositions,
        bool HasRfStatus,
        string? OperationalZone);
}
