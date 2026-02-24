using FluentAssertions;
using TelecomPM.Domain.Entities.Visits;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.ValueObjects;
using TelecomPM.Infrastructure.Services;
using Xunit;

namespace TelecomPM.Infrastructure.Tests.Services;

public class GeoCheckInServiceTests
{
    [Fact]
    public void CheckIn_Within200Meters_ShouldSetWithinRadiusTrue()
    {
        var visit = CreateVisit();
        var siteLocation = GeoLocation.Create(30.1000m, 31.2000m);
        var engineerLocation = GeoLocation.Create(30.1005m, 31.2005m);
        var sut = new GeoCheckInService();

        var result = sut.CheckIn(visit, engineerLocation, siteLocation, 200m);

        result.IsWithinSiteRadius.Should().BeTrue();
        result.DistanceFromSiteMeters.Should().BeGreaterThan(0m);
        result.DistanceFromSiteMeters.Should().BeLessThan(200m);
        visit.IsWithinSiteRadius.Should().BeTrue();
    }

    [Fact]
    public void CheckIn_At500Meters_ShouldBeAllowedButFlagged()
    {
        var visit = CreateVisit();
        var siteLocation = GeoLocation.Create(30.1000m, 31.2000m);
        var engineerLocation = GeoLocation.Create(30.1045m, 31.2000m);
        var sut = new GeoCheckInService();

        var result = sut.CheckIn(visit, engineerLocation, siteLocation, 200m);

        result.IsWithinSiteRadius.Should().BeFalse();
        result.DistanceFromSiteMeters.Should().BeGreaterThan(400m);
        visit.CheckInTimeUtc.Should().NotBeNull();
    }

    [Fact]
    public void DistanceTo_Haversine_ShouldReturnKnownRangeInMeters()
    {
        var cairo = GeoLocation.Create(30.0444m, 31.2357m);
        var giza = GeoLocation.Create(29.9765m, 31.1313m);

        var distance = cairo.DistanceTo(giza);

        distance.Should().BeInRange(5000m, 20000m);
    }

    [Fact]
    public void CheckIn_OutsideRadius_ShouldRaiseSuspiciousCheckInEvent()
    {
        var visit = CreateVisit();
        var siteLocation = GeoLocation.Create(30.1000m, 31.2000m);
        var engineerLocation = GeoLocation.Create(30.1045m, 31.2000m);
        var sut = new GeoCheckInService();

        sut.CheckIn(visit, engineerLocation, siteLocation, 200m);

        visit.DomainEvents.Should().Contain(e => e.GetType().Name == "SuspiciousCheckInEvent");
    }

    private static Visit CreateVisit()
        => Visit.Create(
            "V-GPS-001",
            Guid.NewGuid(),
            "CAI001",
            "Cairo Site",
            Guid.NewGuid(),
            "Engineer A",
            DateTime.UtcNow.AddMinutes(30),
            VisitType.BM);
}
