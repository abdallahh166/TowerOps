using FluentAssertions;
using TowerOps.Domain.Entities.Materials;
using TowerOps.Domain.Enums;
using TowerOps.Domain.Exceptions;
using TowerOps.Domain.ValueObjects;

namespace TowerOps.Domain.Tests.Entities;

public class MaterialEdgeTests
{
    private static Material CreateMaterial()
    {
        return Material.Create(
            code: "CAB001",
            name: "Cable 1m",
            description: "Patch cable",
            category: MaterialCategory.Transmission,
            officeId: Guid.NewGuid(),
            initialStock: MaterialQuantity.Create(2, MaterialUnit.Pieces),
            minimumStock: MaterialQuantity.Create(1, MaterialUnit.Pieces),
            unitCost: Money.Create(50, "EGP")
        );
    }

    [Fact]
    public void ReserveBeyondAvailable_ShouldThrow()
    {
        var m = CreateMaterial();
        Action act = () => m.ReserveStock(MaterialQuantity.Create(5, MaterialUnit.Pieces), Guid.NewGuid());
        act.Should().Throw<DomainException>();
    }
}


