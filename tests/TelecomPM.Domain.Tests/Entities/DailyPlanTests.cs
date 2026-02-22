using FluentAssertions;
using TelecomPM.Domain.Entities.DailyPlans;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Exceptions;
using TelecomPM.Domain.ValueObjects;
using Xunit;

namespace TelecomPM.Domain.Tests.Entities;

public class DailyPlanTests
{
    [Fact]
    public void SuggestOrder_ShouldPlaceP1First()
    {
        var plan = DailyPlan.Create(Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow.Date), Guid.NewGuid());
        var engineerId = Guid.NewGuid();

        plan.AssignSiteToEngineer(engineerId, "CAI001", GeoLocation.Create(30.1000m, 31.2000m), VisitType.CM, "P2");
        plan.AssignSiteToEngineer(engineerId, "CAI002", GeoLocation.Create(30.1100m, 31.2100m), VisitType.CM, "P1");

        var order = plan.SuggestOrder(engineerId, 40m);

        order.Should().NotBeEmpty();
        order.First().Priority.Should().Be("P1");
    }

    [Fact]
    public void SuggestOrder_ShouldUseNearestNeighborWithinSamePriority()
    {
        var plan = DailyPlan.Create(Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow.Date), Guid.NewGuid());
        var engineerId = Guid.NewGuid();

        // Same priority bucket. First is CAI001, then closest should be CAI003, not CAI002.
        plan.AssignSiteToEngineer(engineerId, "CAI001", GeoLocation.Create(30.1000m, 31.2000m), VisitType.CM, "P2");
        plan.AssignSiteToEngineer(engineerId, "CAI002", GeoLocation.Create(30.2000m, 31.3000m), VisitType.CM, "P2");
        plan.AssignSiteToEngineer(engineerId, "CAI003", GeoLocation.Create(30.1010m, 31.2010m), VisitType.CM, "P2");

        var order = plan.SuggestOrder(engineerId, 40m);

        order.Select(s => s.SiteCode).Should().ContainInOrder("CAI001", "CAI003");
    }

    [Fact]
    public void Publish_ShouldPreventFurtherModifications()
    {
        var plan = DailyPlan.Create(Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow.Date), Guid.NewGuid());
        plan.Publish();

        var act = () => plan.AssignSiteToEngineer(
            Guid.NewGuid(),
            "CAI001",
            GeoLocation.Create(30.1m, 31.2m),
            VisitType.CM,
            "P2");

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void AssignSiteToEngineer_ShouldAllowReassignmentBetweenEngineers()
    {
        var plan = DailyPlan.Create(Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow.Date), Guid.NewGuid());
        var engineerA = Guid.NewGuid();
        var engineerB = Guid.NewGuid();

        plan.AssignSiteToEngineer(engineerA, "CAI001", GeoLocation.Create(30.1m, 31.2m), VisitType.CM, "P2");
        plan.AssignSiteToEngineer(engineerB, "CAI001", GeoLocation.Create(30.1m, 31.2m), VisitType.CM, "P2");

        var planA = plan.EngineerPlans.First(x => x.EngineerId == engineerA);
        var planB = plan.EngineerPlans.First(x => x.EngineerId == engineerB);

        planA.Stops.Should().BeEmpty();
        planB.Stops.Should().ContainSingle(s => s.SiteCode == "CAI001");
    }
}
