using FluentAssertions;
using TelecomPM.Domain.Entities.Materials;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.ValueObjects;

namespace TelecomPM.Domain.Tests.Entities;

public class MaterialReorderSupplierTests
{
    [Fact]
    public void ReorderAndSupplierUpdates_ShouldPersist()
    {
        var m = Material.Create(
            code: "CAB002",
            name: "Cable 2m",
            description: "Patch cable",
            category: MaterialCategory.Transmission,
            officeId: Guid.NewGuid(),
            initialStock: MaterialQuantity.Create(10, MaterialUnit.Pieces),
            minimumStock: MaterialQuantity.Create(2, MaterialUnit.Pieces),
            unitCost: Money.Create(75, "EGP")
        );

        m.SetReorderQuantity(MaterialQuantity.Create(5, MaterialUnit.Pieces));
        m.SetSupplier("El Sewedy");

        m.ReorderQuantity!.Value.Should().Be(5);
        m.Supplier.Should().Be("El Sewedy");
    }
}


