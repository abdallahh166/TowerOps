using ClosedXML.Excel;
using FluentAssertions;
using MediatR;
using Moq;
using TelecomPM.Application.Commands.Reports.GenerateContractorScorecard;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Kpi;
using TelecomPM.Application.Queries.Kpi.GetOperationsDashboard;
using TelecomPM.Domain.Entities.Offices;
using TelecomPM.Domain.Entities.Sites;
using TelecomPM.Domain.Entities.Visits;
using TelecomPM.Domain.Entities.WorkOrders;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Interfaces.Repositories;
using TelecomPM.Domain.Specifications;
using TelecomPM.Domain.ValueObjects;
using Xunit;

namespace TelecomPM.Application.Tests.Commands.Reports;

public class GenerateContractorScorecardCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnNonEmptyBytes()
    {
        var sut = CreateSut(out _, out _, out _, out _, out _, out _);
        var result = await sut.Handle(new GenerateContractorScorecardCommand { OfficeCode = "CAI", Month = 2, Year = 2026 }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Length.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Handle_ShouldContainExpectedSheetNames()
    {
        var sut = CreateSut(out _, out _, out _, out _, out _, out _);
        var result = await sut.Handle(new GenerateContractorScorecardCommand { OfficeCode = "CAI", Month = 2, Year = 2026 }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        using var stream = new MemoryStream(result.Value!);
        using var workbook = new XLWorkbook(stream);
        workbook.Worksheets.Select(w => w.Name).Should().Contain(new[] { "Summary", "WO Details", "Evidence Details" });
    }

    private static GenerateContractorScorecardCommandHandler CreateSut(
        out Mock<IOfficeRepository> officeRepo,
        out Mock<ISiteRepository> siteRepo,
        out Mock<IWorkOrderRepository> workOrderRepo,
        out Mock<IVisitRepository> visitRepo,
        out Mock<ISender> sender,
        out Office office)
    {
        office = Office.Create("CAI", "Cairo Office", "Cairo", Address.Create("Street", "Cairo", "Cairo"));
        var site = Site.Create("CAI001", "Site A", "OMC", office.Id, "R", "SR", Coordinates.Create(30.1, 31.2), Address.Create("S", "C", "R"), SiteType.Macro);

        var wo = WorkOrder.Create("WO-1", "CAI001", "CAI", SlaClass.P2, "Issue");
        wo.Assign(Guid.NewGuid(), "Eng", "Disp");
        wo.Start();
        wo.Complete();
        wo.Close();

        var visit = Visit.Create("V-1", site.Id, site.SiteCode.Value, site.Name, Guid.NewGuid(), "Eng 1", DateTime.UtcNow, VisitType.BM);
        visit.AddPhoto(VisitPhoto.Create(visit.Id, PhotoType.Before, PhotoCategory.ShelterInside, "Item1", "p1.jpg", "/p1.jpg"));
        visit.AddPhoto(VisitPhoto.Create(visit.Id, PhotoType.After, PhotoCategory.ShelterOutside, "Item2", "p2.jpg", "/p2.jpg"));
        visit.AddReading(VisitReading.Create(visit.Id, "Rectifier DC Voltage", "Power", 53m, "V"));
        var checklist = VisitChecklist.Create(visit.Id, "Power", "Check", "desc");
        checklist.UpdateStatus(CheckStatus.OK);
        visit.AddChecklistItem(checklist);

        officeRepo = new Mock<IOfficeRepository>();
        var localOffice = office;
        officeRepo.Setup(x => x.GetByCodeAsNoTrackingAsync("CAI", It.IsAny<CancellationToken>())).ReturnsAsync(localOffice);

        siteRepo = new Mock<ISiteRepository>();
        siteRepo.Setup(x => x.GetByOfficeIdAsNoTrackingAsync(localOffice.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Site> { site });

        workOrderRepo = new Mock<IWorkOrderRepository>();
        workOrderRepo.Setup(x => x.FindAsNoTrackingAsync(It.IsAny<ISpecification<WorkOrder>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<WorkOrder> { wo });

        visitRepo = new Mock<IVisitRepository>();
        visitRepo.Setup(x => x.FindAsNoTrackingAsync(It.IsAny<ISpecification<Visit>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Visit> { visit });

        sender = new Mock<ISender>();
        sender.Setup(x => x.Send(It.IsAny<GetOperationsDashboardQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(new OperationsKpiDashboardDto
            {
                SlaCompliancePercentage = 90m,
                FtfRatePercent = 80m,
                MttrHours = 6.5m,
                ReopenRatePercent = 10m,
                EvidenceCompletenessPercent = 85m
            }));

        return new GenerateContractorScorecardCommandHandler(
            officeRepo.Object,
            siteRepo.Object,
            workOrderRepo.Object,
            visitRepo.Object,
            sender.Object);
    }
}
