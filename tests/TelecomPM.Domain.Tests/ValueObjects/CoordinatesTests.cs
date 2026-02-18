using FluentAssertions;
using TelecomPM.Domain.Exceptions;
using TelecomPM.Domain.ValueObjects;
using Xunit;

namespace TelecomPM.Domain.Tests.ValueObjects;

public class CoordinatesTests
{
    [Fact]
    public void Create_WithValidLatLon_ShouldSucceed()
    {
        var coords = Coordinates.Create(30.0, 31.0);
        coords.Latitude.Should().Be(30.0);
        coords.Longitude.Should().Be(31.0);
    }

    [Theory]
    [InlineData(-91, 0)]
    [InlineData(91, 0)]
    [InlineData(0, -181)]
    [InlineData(0, 181)]
    public void Create_WithOutOfRange_ShouldThrow(double lat, double lon)
    {
        var act = () => Coordinates.Create(lat, lon);
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void DistanceTo_ShouldComputePositiveDistance()
    {
        var a = Coordinates.Create(30.0444, 31.2357); // Cairo
        var b = Coordinates.Create(29.9765, 31.1313); // Giza

        var km = a.DistanceTo(b);

        km.Should().BeGreaterThan(0);
        km.Should().BeInRange(5, 20);
    }
}


