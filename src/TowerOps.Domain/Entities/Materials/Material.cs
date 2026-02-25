using TowerOps.Domain.Common;
using TowerOps.Domain.Enums;
using TowerOps.Domain.Events.MaterialEvents;
using TowerOps.Domain.Exceptions;
using TowerOps.Domain.ValueObjects;

namespace TowerOps.Domain.Entities.Materials;

// ==================== Material (Aggregate Root) ====================
public sealed class Material : AggregateRoot<Guid>
{
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public MaterialCategory Category { get; private set; }
    public Guid OfficeId { get; private set; }

    // Transactions
    private readonly List<MaterialTransaction> _transactions = new();
    public IReadOnlyList<MaterialTransaction> Transactions => _transactions.AsReadOnly();


    // Stock Management
    public MaterialQuantity CurrentStock { get; private set; } = null!;
    public MaterialQuantity MinimumStock { get; private set; } = null!;
    public MaterialQuantity? ReorderQuantity { get; private set; }
    
    // Pricing
    public Money UnitCost { get; private set; } = null!;

    // Tracking
    public string? Supplier { get; private set; }
    public DateTime? LastRestockDate { get; private set; }
    public bool IsActive { get; private set; }

    //reserved for EF Core
    private readonly List<MaterialReservation> _reservations = new();
    public IReadOnlyCollection<MaterialReservation> Reservations => _reservations.AsReadOnly();

    private Material() : base() { }

    private Material(
        string code,
        string name,
        string description,
        MaterialCategory category,
        Guid officeId,
        MaterialQuantity initialStock,
        MaterialQuantity minimumStock,
        Money unitCost) : base(Guid.NewGuid())
    {
        Code = code;
        Name = name;
        Description = description;
        Category = category;
        OfficeId = officeId;
        CurrentStock = initialStock;
        MinimumStock = minimumStock;
        UnitCost = unitCost;
        IsActive = true;
    }

    public static Material Create(
        string code,
        string name,
        string description,
        MaterialCategory category,
        Guid officeId,
        MaterialQuantity initialStock,
        MaterialQuantity minimumStock,
        Money unitCost)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("Material code is required", "Material.Code.Required");

        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Material name is required", "Material.Name.Required");

        return new Material(
            code.ToUpper(), 
            name, 
            description, 
            category, 
            officeId, 
            initialStock, 
            minimumStock, 
            unitCost);
    }

    public void UpdateInfo(string name, string description, MaterialCategory category)
    {
        Name = name;
        Description = description;
        Category = category;
        MarkAsUpdated("System");
    }

    public void UpdatePricing(Money unitCost)
    {
        UnitCost = unitCost;
        MarkAsUpdated("System");
    }

    public void SetReorderQuantity(MaterialQuantity quantity)
    {
        ReorderQuantity = quantity;
    }

    public void SetSupplier(string supplier)
    {
        Supplier = supplier;
    }

    public void AddStock(MaterialQuantity quantity, string? restockedBy = null)
    {
        var before = CurrentStock;
        CurrentStock = CurrentStock.Add(quantity);
        LastRestockDate = DateTime.UtcNow;
        MarkAsUpdated("System");

        _transactions.Add(MaterialTransaction.Create(
            Id,
            TransactionType.Purchase,
            quantity,
            before,
            CurrentStock,
            "Stock added",
            restockedBy ?? "System"
        ));

        // Raise MaterialRestockedEvent
        AddDomainEvent(new MaterialRestockedEvent(
            Id,
            Name,
            OfficeId,
            quantity.Value,
            quantity.Unit.ToString(),
            before.Value,
            CurrentStock.Value,
            restockedBy ?? "System",
            DateTime.UtcNow));
    }


    public void DeductStock(MaterialQuantity quantity)
    {
        var before = CurrentStock;
        CurrentStock = CurrentStock.Subtract(quantity);
        MarkAsUpdated("System");

        _transactions.Add(MaterialTransaction.Create(
            Id,
            TransactionType.Usage,
            quantity,
            before,
            CurrentStock,
            "Stock deducted",
            "System"
        ));

        if (IsStockLow())
        {
            AddDomainEvent(new LowStockAlertEvent(
                Id,
                Name,
                OfficeId,
                CurrentStock.Value,
                MinimumStock.Value));
        }
    }


    public void AdjustStock(MaterialQuantity newQuantity, string reason)
    {
        var before = CurrentStock;
        CurrentStock = newQuantity;
        MarkAsUpdated($"Stock adjusted: {reason}");

        _transactions.Add(MaterialTransaction.Create(
            Id,
            TransactionType.Adjustment,
            newQuantity,
            before,
            CurrentStock,
            reason,
            "System"
        ));
    }

    public void TransferStock(MaterialQuantity quantity, string reason, string performedBy)
    {
        var before = CurrentStock;
        CurrentStock = CurrentStock.Subtract(quantity);
        MarkAsUpdated($"Stock transferred: {reason}");

        _transactions.Add(MaterialTransaction.Create(
            Id,
            TransactionType.Transfer,
            quantity,
            before,
            CurrentStock,
            reason,
            performedBy
        ));

        if (IsStockLow())
        {
            AddDomainEvent(new LowStockAlertEvent(
                Id,
                Name,
                OfficeId,
                CurrentStock.Value,
                MinimumStock.Value));
        }
    }

    public void UpdateMinimumStock(MaterialQuantity newMinimum)
    {
        MinimumStock = newMinimum;
    }

    public bool IsStockLow()
    {
        return CurrentStock.Value <= MinimumStock.Value;
    }

    public bool IsStockAvailable(MaterialQuantity requestedQuantity)
    {
        return CurrentStock.Value >= requestedQuantity.Value && 
               CurrentStock.Unit == requestedQuantity.Unit;
    }

    public void ReserveStock(MaterialQuantity quantity, Guid visitId)
    {
        if (!IsStockAvailable(quantity))
            throw new DomainException(
                $"Insufficient stock for material {Name}",
                "Material.Stock.Insufficient",
                Name);

        _reservations.Add(new MaterialReservation(Id, visitId, quantity));
    }

    public void ConsumeStock(Guid visitId, string performedBy)
    {
        var reservations = _reservations.Where(r => r.VisitId == visitId && !r.IsConsumed).ToList();

        foreach (var reservation in reservations)
        {
            DeductStock(reservation.Quantity);
            reservation.MarkAsConsumed();

            AddDomainEvent(new MaterialConsumedEvent(
                Id,
                Name,
                visitId,
                reservation.Quantity.Value,
                reservation.Quantity.Unit.ToString(),   
                performedBy,
                DateTime.UtcNow
            ));
        }
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}
