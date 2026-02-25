using FluentAssertions;
using Moq;
using TowerOps.Application.Queries.DailyPlans.GetUnassignedSites;
using TowerOps.Domain.Entities.DailyPlans;
using TowerOps.Domain.Entities.Sites;
using TowerOps.Domain.Enums;
using TowerOps.Domain.Interfaces.Repositories;
using TowerOps.Domain.ValueObjects;
using Xunit;

namespace TowerOps.Application.Tests.Queries.DailyPlans;

public class GetUnassignedSitesQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnSitesNotAssignedInPlan()
    {
        var officeId = Guid.NewGuid();
        var date = DateOnly.FromDateTime(DateTime.UtcNow.Date);

        var plan = DailyPlan.Create(officeId, date, Guid.NewGuid());
        plan.AssignSiteToEngineer(Guid.NewGuid(), "CAI001", GeoLocation.Create(30.1m, 31.2m), VisitType.CM, "P2");

        var sites = new List<Site>
        {
            CreateSite("CAI001", officeId),
            CreateSite("CAI002", officeId),
            CreateSite("CAI003", officeId)
        };

        var dailyPlanRepo = new Mock<IDailyPlanRepository>();
        dailyPlanRepo
            .Setup(r => r.GetByOfficeAndDateAsNoTrackingAsync(officeId, date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(plan);

        var siteRepo = new Mock<ISiteRepository>();
        siteRepo
            .Setup(r => r.GetByOfficeIdAsNoTrackingAsync(officeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sites);

        var sut = new GetUnassignedSitesQueryHandler(dailyPlanRepo.Object, siteRepo.Object);
        var result = await sut.Handle(new GetUnassignedSitesQuery { OfficeId = officeId, Date = date }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value!.Select(x => x.SiteCode).Should().BeEquivalentTo(new[] { "CAI002", "CAI003" });
    }

    private static Site CreateSite(string siteCode, Guid officeId)
    {
        return Site.Create(
            siteCode,
            $"Site {siteCode}",
            "OMC",
            officeId,
            "Cairo",
            "East",
            Coordinates.Create(30.1, 31.2),
            Address.Create("Street", "City", "Region"),
            SiteType.Macro);
    }
}
