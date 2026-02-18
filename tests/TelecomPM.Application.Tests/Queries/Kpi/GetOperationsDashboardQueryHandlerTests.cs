using FluentAssertions;
using Moq;
using TelecomPM.Application.Queries.Kpi.GetOperationsDashboard;
using TelecomPM.Domain.Entities.Visits;
using TelecomPM.Domain.Entities.WorkOrders;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Interfaces.Repositories;
using Xunit;

namespace TelecomPM.Application.Tests.Queries.Kpi;

public class GetOperationsDashboardQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldApplyOfficeAndSlaFilters()
    {
        var target = WorkOrder.Create("WO-1", "S-1", "CAI", SlaClass.P1, "Issue1");
        var other = WorkOrder.Create("WO-2", "S-2", "ALX", SlaClass.P3, "Issue2");

        var workOrderRepo = new Mock<IWorkOrderRepository>();
        workOrderRepo.Setup(x => x.GetAllAsNoTrackingAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<WorkOrder> { target, other });

        var visitRepo = new Mock<IVisitRepository>();
        visitRepo.Setup(x => x.GetAllAsNoTrackingAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Visit>());

        var sut = new GetOperationsDashboardQueryHandler(workOrderRepo.Object, visitRepo.Object);

        var result = await sut.Handle(new GetOperationsDashboardQuery
        {
            OfficeCode = "CAI",
            SlaClass = SlaClass.P1
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.TotalWorkOrders.Should().Be(1);
        result.Value.OfficeCode.Should().Be("CAI");
        result.Value.SlaClass.Should().Be(SlaClass.P1);
    }

    [Fact]
    public async Task Handle_ShouldApplyDateRangeFilter()
    {
        var wo = WorkOrder.Create("WO-DATE", "S-10", "CAI", SlaClass.P2, "Issue");

        var workOrderRepo = new Mock<IWorkOrderRepository>();
        workOrderRepo.Setup(x => x.GetAllAsNoTrackingAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<WorkOrder> { wo });

        var visitRepo = new Mock<IVisitRepository>();
        visitRepo.Setup(x => x.GetAllAsNoTrackingAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Visit>());

        var sut = new GetOperationsDashboardQueryHandler(workOrderRepo.Object, visitRepo.Object);

        var excluded = await sut.Handle(new GetOperationsDashboardQuery
        {
            FromDateUtc = DateTime.UtcNow.AddDays(1),
            ToDateUtc = DateTime.UtcNow.AddDays(2)
        }, CancellationToken.None);

        excluded.Value!.TotalWorkOrders.Should().Be(0);
    }
}
