using FluentAssertions;
using TowerOps.Domain.Enums;
using TowerOps.Domain.Exceptions;
using TowerOps.Domain.ValueObjects;
using Xunit;

namespace TowerOps.Domain.Tests.ValueObjects;

public class MaterialQuantityTests
{
    [Fact]
    public void Create_WithPositiveValue_ShouldSucceed()
    {
        var q = MaterialQuantity.Create(10, MaterialUnit.Pieces);
        q.Value.Should().Be(10);
        q.Unit.Should().Be(MaterialUnit.Pieces);
    }

    [Fact]
    public void Add_WithSameUnit_ShouldSum()
    {
        var a = MaterialQuantity.Create(5, MaterialUnit.Meters);
        var b = MaterialQuantity.Create(3, MaterialUnit.Meters);
        a.Add(b).Value.Should().Be(8);
    }

    [Fact]
    public void Add_WithDifferentUnit_ShouldThrow()
    {
        var a = MaterialQuantity.Create(5, MaterialUnit.Meters);
        var b = MaterialQuantity.Create(3, MaterialUnit.Pieces);
        var act = () => a.Add(b);
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Comparison_Operators_ShouldWorkForSameUnit()
    {
        var a = MaterialQuantity.Create(5, MaterialUnit.Meters);
        var b = MaterialQuantity.Create(7, MaterialUnit.Meters);
        (a < b).Should().BeTrue();
        (b > a).Should().BeTrue();
        (a <= b).Should().BeTrue();
        (b >= a).Should().BeTrue();
    }
}


