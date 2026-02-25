using TowerOps.Domain.Common;
using TowerOps.Domain.ValueObjects;

namespace TowerOps.Domain.Entities.Materials;

// ==================== Material Transaction (Entity) ====================
public sealed class MaterialTransaction : Entity<Guid>
{
    public Guid MaterialId { get; private set; }
    public Guid? VisitId { get; private set; }
    public TransactionType Type { get; private set; }
    public MaterialQuantity Quantity { get; private set; } = null!;
    public MaterialQuantity StockBefore { get; private set; } = null!;
    public MaterialQuantity StockAfter { get; private set; } = null!;
    public string Reason { get; private set; } = string.Empty;
    public DateTime TransactionDate { get; private set; }
    public string PerformedBy { get; private set; } = string.Empty;

    private MaterialTransaction() : base() { }

    private MaterialTransaction(
        Guid materialId,
        TransactionType type,
        MaterialQuantity quantity,
        MaterialQuantity stockBefore,
        MaterialQuantity stockAfter,
        string reason,
        string performedBy,
        Guid? visitId = null) : base(Guid.NewGuid())
    {
        MaterialId = materialId;
        Type = type;
        Quantity = quantity;
        StockBefore = stockBefore;
        StockAfter = stockAfter;
        Reason = reason;
        PerformedBy = performedBy;
        VisitId = visitId;
        TransactionDate = DateTime.UtcNow;
    }

    public static MaterialTransaction Create(
        Guid materialId,
        TransactionType type,
        MaterialQuantity quantity,
        MaterialQuantity stockBefore,
        MaterialQuantity stockAfter,
        string reason,
        string performedBy,
        Guid? visitId = null)
    {
        return new MaterialTransaction(
            materialId, 
            type, 
            quantity, 
            stockBefore, 
            stockAfter, 
            reason, 
            performedBy, 
            visitId);
    }
}

public enum TransactionType
{
    Purchase = 1,
    Usage = 2,
    Adjustment = 3,
    Transfer = 4,
    Return = 5
}
