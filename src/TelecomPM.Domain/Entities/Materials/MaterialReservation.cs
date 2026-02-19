using TelecomPM.Domain.ValueObjects;

namespace TelecomPM.Domain.Entities.Materials;

public sealed class MaterialReservation
{
    public Guid Id { get; private set; }
    public Guid MaterialId { get; private set; }
    public Guid VisitId { get; private set; }
    public MaterialQuantity Quantity { get; private set; } = null!;
    public DateTime ReservedAt { get; private set; }
    public bool IsConsumed { get; private set; }

    private MaterialReservation() { } // EF Core

    public MaterialReservation(Guid materialId, Guid visitId, MaterialQuantity quantity)
    {
        Id = Guid.NewGuid();
        MaterialId = materialId;
        VisitId = visitId;
        Quantity = quantity;
        ReservedAt = DateTime.UtcNow;
        IsConsumed = false;
    }

    public void MarkAsConsumed()
    {
        IsConsumed = true;
    }
}
