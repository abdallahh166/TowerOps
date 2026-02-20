using FluentAssertions;
using Moq;
using TelecomPM.Application.Queries.Kpi.GetOperationsDashboard;
using TelecomPM.Domain.Entities.Visits;
using TelecomPM.Domain.Entities.WorkOrders;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Interfaces.Repositories;
using TelecomPM.Domain.Specifications;
using Xunit;

namespace TelecomPM.Application.Tests.Queries.Kpi;

public class GetOperationsDashboardQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldCalculateFtfRatePercent()
    {
        var workOrderRepo = CreateWorkOrderRepoForKpiTests(
            totalWorkOrders: 20,
            openWorkOrders: 5,
            closedWorkOrders: 15,
            breachedWorkOrders: 2,
            atRiskWorkOrders: 1,
            closedWithReworkOrReopened: 3,
            reopenedClosedWorkOrders: 2,
            mttrHours: 6.5m);

        var visitRepo = CreateVisitRepoForKpiTests(
            reviewedVisits: 10,
            submittedVisits: 8,
            evidenceCompleteSubmittedVisits: 6);

        var sut = new GetOperationsDashboardQueryHandler(workOrderRepo.Object, visitRepo.Object);

        var result = await sut.Handle(new GetOperationsDashboardQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.FtfRatePercent.Should().Be(80m); // (15 - 3) / 15 * 100
    }

    [Fact]
    public async Task Handle_ShouldCalculateMttrHours()
    {
        var workOrderRepo = CreateWorkOrderRepoForKpiTests(
            totalWorkOrders: 10,
            openWorkOrders: 2,
            closedWorkOrders: 8,
            breachedWorkOrders: 1,
            atRiskWorkOrders: 0,
            closedWithReworkOrReopened: 1,
            reopenedClosedWorkOrders: 1,
            mttrHours: 4.25m);

        var visitRepo = CreateVisitRepoForKpiTests(
            reviewedVisits: 4,
            submittedVisits: 4,
            evidenceCompleteSubmittedVisits: 3);

        var sut = new GetOperationsDashboardQueryHandler(workOrderRepo.Object, visitRepo.Object);

        var result = await sut.Handle(new GetOperationsDashboardQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.MttrHours.Should().Be(4.25m);
    }

    [Fact]
    public async Task Handle_ShouldCalculateReopenRatePercent()
    {
        var workOrderRepo = CreateWorkOrderRepoForKpiTests(
            totalWorkOrders: 12,
            openWorkOrders: 2,
            closedWorkOrders: 10,
            breachedWorkOrders: 1,
            atRiskWorkOrders: 1,
            closedWithReworkOrReopened: 2,
            reopenedClosedWorkOrders: 3,
            mttrHours: 5m);

        var visitRepo = CreateVisitRepoForKpiTests(
            reviewedVisits: 6,
            submittedVisits: 5,
            evidenceCompleteSubmittedVisits: 4);

        var sut = new GetOperationsDashboardQueryHandler(workOrderRepo.Object, visitRepo.Object);

        var result = await sut.Handle(new GetOperationsDashboardQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.ReopenRatePercent.Should().Be(30m); // 3 / 10 * 100
    }

    [Fact]
    public async Task Handle_ShouldCalculateEvidenceCompletenessPercent()
    {
        var workOrderRepo = CreateWorkOrderRepoForKpiTests(
            totalWorkOrders: 8,
            openWorkOrders: 3,
            closedWorkOrders: 5,
            breachedWorkOrders: 1,
            atRiskWorkOrders: 1,
            closedWithReworkOrReopened: 1,
            reopenedClosedWorkOrders: 0,
            mttrHours: 2.5m);

        var visitRepo = CreateVisitRepoForKpiTests(
            reviewedVisits: 5,
            submittedVisits: 7,
            evidenceCompleteSubmittedVisits: 4);

        var sut = new GetOperationsDashboardQueryHandler(workOrderRepo.Object, visitRepo.Object);

        var result = await sut.Handle(new GetOperationsDashboardQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.EvidenceCompletenessPercent.Should().Be(57.14m); // 4 / 7 * 100 rounded
    }

    [Fact]
    public async Task Handle_ShouldUseCountQueriesAndNotLoadFullDatasets()
    {
        var workOrders = new List<WorkOrder>
        {
            WorkOrder.Create("WO-DATE", "S-10", "CAI", SlaClass.P2, "Issue")
        };
        var visits = new List<Visit> { CreateVisit() };

        var workOrderRepo = new Mock<IWorkOrderRepository>();
        workOrderRepo.Setup(x => x.CountAsync(It.IsAny<ISpecification<WorkOrder>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ISpecification<WorkOrder> spec, CancellationToken _) => Apply(spec, workOrders).Count());
        workOrderRepo.Setup(x => x.CountClosedAsync(It.IsAny<string?>(), It.IsAny<SlaClass?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        workOrderRepo.Setup(x => x.CountClosedWithReworkOrReopenedHistoryAsync(It.IsAny<string?>(), It.IsAny<SlaClass?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
        workOrderRepo.Setup(x => x.CountClosedWithReopenedHistoryAsync(It.IsAny<string?>(), It.IsAny<SlaClass?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
        workOrderRepo.Setup(x => x.GetClosedMeanTimeToRepairHoursAsync(It.IsAny<string?>(), It.IsAny<SlaClass?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1.5m);

        var visitRepo = new Mock<IVisitRepository>();
        visitRepo.Setup(x => x.CountAsync(It.IsAny<ISpecification<Visit>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ISpecification<Visit> spec, CancellationToken _) => Apply(spec, visits).Count());

        var sut = new GetOperationsDashboardQueryHandler(workOrderRepo.Object, visitRepo.Object);

        var result = await sut.Handle(new GetOperationsDashboardQuery
        {
            FromDateUtc = DateTime.UtcNow.AddDays(1),
            ToDateUtc = DateTime.UtcNow.AddDays(2)
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        workOrderRepo.Verify(x => x.GetAllAsNoTrackingAsync(It.IsAny<CancellationToken>()), Times.Never);
        visitRepo.Verify(x => x.GetAllAsNoTrackingAsync(It.IsAny<CancellationToken>()), Times.Never);
        visitRepo.Verify(x => x.FindAsNoTrackingAsync(It.IsAny<ISpecification<Visit>>(), It.IsAny<CancellationToken>()), Times.Never);
        workOrderRepo.Verify(x => x.CountAsync(It.IsAny<ISpecification<WorkOrder>>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
        visitRepo.Verify(x => x.CountAsync(It.IsAny<ISpecification<Visit>>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
    }

    private static Mock<IWorkOrderRepository> CreateWorkOrderRepoForKpiTests(
        int totalWorkOrders,
        int openWorkOrders,
        int closedWorkOrders,
        int breachedWorkOrders,
        int atRiskWorkOrders,
        int closedWithReworkOrReopened,
        int reopenedClosedWorkOrders,
        decimal mttrHours)
    {
        var repo = new Mock<IWorkOrderRepository>();

        repo.Setup(x => x.CountAsync(It.IsAny<ISpecification<WorkOrder>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ISpecification<WorkOrder> spec, CancellationToken _) =>
            {
                if (spec.Criteria is null)
                    return totalWorkOrders;

                var text = spec.Criteria.ToString();
                if (text.Contains("Status") && text.Contains("Closed") && text.Contains("Cancelled"))
                    return openWorkOrders;
                if (text.Contains("ResolutionDeadlineUtc") && text.Contains("<"))
                    return breachedWorkOrders;
                if (text.Contains("ResolutionDeadlineUtc") && text.Contains("AddHours"))
                    return atRiskWorkOrders;
                return totalWorkOrders;
            });

        repo.Setup(x => x.CountClosedAsync(It.IsAny<string?>(), It.IsAny<SlaClass?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(closedWorkOrders);
        repo.Setup(x => x.CountClosedWithReworkOrReopenedHistoryAsync(It.IsAny<string?>(), It.IsAny<SlaClass?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(closedWithReworkOrReopened);
        repo.Setup(x => x.CountClosedWithReopenedHistoryAsync(It.IsAny<string?>(), It.IsAny<SlaClass?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(reopenedClosedWorkOrders);
        repo.Setup(x => x.GetClosedMeanTimeToRepairHoursAsync(It.IsAny<string?>(), It.IsAny<SlaClass?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mttrHours);

        return repo;
    }

    private static Mock<IVisitRepository> CreateVisitRepoForKpiTests(
        int reviewedVisits,
        int submittedVisits,
        int evidenceCompleteSubmittedVisits)
    {
        var repo = new Mock<IVisitRepository>();
        repo.SetupSequence(x => x.CountAsync(It.IsAny<ISpecification<Visit>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(reviewedVisits)
            .ReturnsAsync(submittedVisits)
            .ReturnsAsync(evidenceCompleteSubmittedVisits);

        return repo;
    }

    private static IEnumerable<T> Apply<T>(ISpecification<T> specification, IEnumerable<T> source)
    {
        if (specification.Criteria is null)
            return source;

        var predicate = specification.Criteria.Compile();
        return source.Where(predicate);
    }

    private static Visit CreateVisit()
    {
        return Visit.Create(
            "V-KPI-1",
            Guid.NewGuid(),
            "S-KPI-1",
            "Site KPI",
            Guid.NewGuid(),
            "Engineer KPI",
            DateTime.UtcNow.AddDays(1),
            VisitType.PreventiveMaintenance);
    }
}
