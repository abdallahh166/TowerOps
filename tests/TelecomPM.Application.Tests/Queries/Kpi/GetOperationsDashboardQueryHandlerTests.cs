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
    public async Task Handle_ShouldApplyOfficeSlaAndDateFiltersViaSpecifications()
    {
        var target = WorkOrder.Create("WO-1", "S-1", "CAI", SlaClass.P1, "Issue1");
        var other = WorkOrder.Create("WO-2", "S-2", "ALX", SlaClass.P3, "Issue2");
        var workOrders = new List<WorkOrder> { target, other };

        var includedVisit = CreateApprovedVisitWithDuration();
        var excludedVisit = CreateApprovedVisitWithDuration();
        var visits = new List<Visit> { includedVisit, excludedVisit };

        var workOrderRepo = new Mock<IWorkOrderRepository>();
        workOrderRepo.Setup(x => x.CountAsync(It.IsAny<ISpecification<WorkOrder>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ISpecification<WorkOrder> spec, CancellationToken _) => Apply(spec, workOrders).Count());

        var visitRepo = new Mock<IVisitRepository>();
        visitRepo.Setup(x => x.CountAsync(It.IsAny<ISpecification<Visit>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ISpecification<Visit> spec, CancellationToken _) => Apply(spec, visits).Count());
        visitRepo.Setup(x => x.FindAsNoTrackingAsync(It.IsAny<ISpecification<Visit>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ISpecification<Visit> spec, CancellationToken _) => (IReadOnlyList<Visit>)Apply(spec, visits).ToList());

        var sut = new GetOperationsDashboardQueryHandler(workOrderRepo.Object, visitRepo.Object);

        var result = await sut.Handle(new GetOperationsDashboardQuery
        {
            OfficeCode = "CAI",
            SlaClass = SlaClass.P1,
            FromDateUtc = DateTime.UtcNow.AddMinutes(-5),
            ToDateUtc = DateTime.UtcNow.AddMinutes(5)
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.TotalWorkOrders.Should().Be(1);
        result.Value.OfficeCode.Should().Be("CAI");
        result.Value.SlaClass.Should().Be(SlaClass.P1);

        workOrderRepo.Verify(x => x.GetAllAsNoTrackingAsync(It.IsAny<CancellationToken>()), Times.Never);
        visitRepo.Verify(x => x.GetAllAsNoTrackingAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldNotLoadMoreRecordsThanNeeded()
    {
        var workOrders = new List<WorkOrder>
        {
            WorkOrder.Create("WO-DATE", "S-10", "CAI", SlaClass.P2, "Issue")
        };
        var visits = new List<Visit> { CreateApprovedVisitWithDuration() };

        var workOrderRepo = new Mock<IWorkOrderRepository>();
        workOrderRepo.Setup(x => x.CountAsync(It.IsAny<ISpecification<WorkOrder>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ISpecification<WorkOrder> spec, CancellationToken _) => Apply(spec, workOrders).Count());

        var visitRepo = new Mock<IVisitRepository>();
        visitRepo.Setup(x => x.CountAsync(It.IsAny<ISpecification<Visit>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ISpecification<Visit> spec, CancellationToken _) => Apply(spec, visits).Count());
        visitRepo.Setup(x => x.FindAsNoTrackingAsync(It.IsAny<ISpecification<Visit>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ISpecification<Visit> spec, CancellationToken _) => (IReadOnlyList<Visit>)Apply(spec, visits).ToList());

        var sut = new GetOperationsDashboardQueryHandler(workOrderRepo.Object, visitRepo.Object);

        var result = await sut.Handle(new GetOperationsDashboardQuery
        {
            FromDateUtc = DateTime.UtcNow.AddDays(1),
            ToDateUtc = DateTime.UtcNow.AddDays(2)
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.TotalWorkOrders.Should().Be(0);

        workOrderRepo.Verify(x => x.GetAllAsNoTrackingAsync(It.IsAny<CancellationToken>()), Times.Never);
        visitRepo.Verify(x => x.GetAllAsNoTrackingAsync(It.IsAny<CancellationToken>()), Times.Never);
        workOrderRepo.Verify(x => x.CountAsync(It.IsAny<ISpecification<WorkOrder>>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
        visitRepo.Verify(x => x.CountAsync(It.IsAny<ISpecification<Visit>>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
    }

    private static IEnumerable<T> Apply<T>(ISpecification<T> specification, IEnumerable<T> source)
    {
        if (specification.Criteria is null)
            return source;

        var predicate = specification.Criteria.Compile();
        return source.Where(predicate);
    }

    private static Visit CreateApprovedVisitWithDuration()
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
