using FluentAssertions;
using TelecomPM.Domain.Entities.Materials;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Exceptions;
using TelecomPM.Domain.ValueObjects;

namespace TelecomPM.Domain.Tests.Entities;

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


