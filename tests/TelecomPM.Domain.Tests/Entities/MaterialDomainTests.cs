using FluentAssertions;
using TelecomPM.Domain.Entities.Materials;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.ValueObjects;

namespace TelecomPM.Domain.Tests.Entities;

public class MaterialDomainTests
{
    private static Material CreateMaterial()
    {
        return Material.Create(
            code: "BAT100",
            name: "Battery 100Ah",
            description: "VRLA Battery",
            category: MaterialCategory.Power,
            officeId: Guid.NewGuid(),
            initialStock: MaterialQuantity.Create(10, MaterialUnit.Pieces),
            minimumStock: MaterialQuantity.Create(5, MaterialUnit.Pieces),
            unitCost: Money.Create(1000, "EGP")
        );
    }

    [Fact]
    public void AddAndDeductStock_ShouldRecordTransactionsAndRaiseLowStockEvent()
    {
        var m = CreateMaterial();
        m.AddStock(MaterialQuantity.Create(5, MaterialUnit.Pieces));
        m.CurrentStock.Value.Should().Be(15);

        m.DeductStock(MaterialQuantity.Create(11, MaterialUnit.Pieces));
        m.CurrentStock.Value.Should().Be(4);
        m.DomainEvents.Should().NotBeEmpty("low stock event should be raised");
    }

    [Fact]
    public void ReserveAndConsumeStock_ShouldDeductAndRaiseConsumedEvent()
    {
        var m = CreateMaterial();
        var visitId = Guid.NewGuid();
        m.ReserveStock(MaterialQuantity.Create(3, MaterialUnit.Pieces), visitId);
        m.ConsumeStock(visitId, "eng");

        m.CurrentStock.Value.Should().Be(7);
        m.DomainEvents.Should().NotBeEmpty();
    }
}


