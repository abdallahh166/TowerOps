using FluentAssertions;
using TowerOps.Domain.Exceptions;
using TowerOps.Domain.ValueObjects;

namespace TowerOps.Domain.Tests.ValueObjects;

public class ValueObjectEdgeTests
{
    [Fact]
    public void Coordinates_OutOfRange_ShouldThrow()
    {
        Action lat = () => Coordinates.Create(100, 30);
        Action lon = () => Coordinates.Create(30, 200);
        lat.Should().Throw<DomainException>();
        lon.Should().Throw<DomainException>();
    }

    [Fact]
    public void Money_SubtractToNegative_ShouldThrow()
    {
        var a = Money.Create(10, "EGP");
        var b = Money.Create(20, "EGP");
        Action act = () => a.Subtract(b);
        act.Should().Throw<DomainException>();
    }
}


