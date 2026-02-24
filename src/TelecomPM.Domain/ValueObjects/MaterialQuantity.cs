using System.Collections.Generic;
using TelecomPM.Domain.Exceptions;
using TelecomPM.Domain.Enums;

namespace TelecomPM.Domain.ValueObjects;

// ==================== Material Quantity ====================
public sealed class MaterialQuantity : ValueObject
{
    public bool IsEmpty => Value <= 0;
    public bool HasStock => Value > 0;

    public decimal Value { get; }
    public MaterialUnit Unit { get; }

    private MaterialQuantity(decimal value, MaterialUnit unit)
    {
        Value = value;
        Unit = unit;
    }

    public static MaterialQuantity Create(decimal value, MaterialUnit unit)
    {
        if (value <= 0)
            throw new DomainException("Material quantity must be greater than zero", "MaterialQuantity.Value.Positive");

        return new MaterialQuantity(value, unit);
    }

    public MaterialQuantity Add(MaterialQuantity other)
    {
        if (Unit != other.Unit)
            throw new DomainException("Cannot add quantities with different units", "MaterialQuantity.Add.UnitMismatch");

        return new MaterialQuantity(Value + other.Value, Unit);
    }

    public MaterialQuantity Subtract(MaterialQuantity other)
    {
        if (Unit != other.Unit)
            throw new DomainException("Cannot subtract quantities with different units", "MaterialQuantity.Subtract.UnitMismatch");

        if (Value < other.Value)
            throw new DomainException("Insufficient quantity", "MaterialQuantity.Subtract.Insufficient");

        return new MaterialQuantity(Value - other.Value, Unit);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
        yield return Unit;
    }

    public override string ToString() => $"{Value} {Unit}";

    public static bool operator <=(MaterialQuantity left, MaterialQuantity right)
    {
        if (left.Unit != right.Unit)
            throw new DomainException("Cannot compare quantities with different units", "MaterialQuantity.Compare.UnitMismatch");
        return left.Value <= right.Value;
    }

    public static bool operator >=(MaterialQuantity left, MaterialQuantity right)
    {
        if (left.Unit != right.Unit)
            throw new DomainException("Cannot compare quantities with different units", "MaterialQuantity.Compare.UnitMismatch");
        return left.Value >= right.Value;
    }

    public static bool operator <(MaterialQuantity left, MaterialQuantity right)
    {
        if (left.Unit != right.Unit)
            throw new DomainException("Cannot compare quantities with different units", "MaterialQuantity.Compare.UnitMismatch");
        return left.Value < right.Value;
    }

    public static bool operator >(MaterialQuantity left, MaterialQuantity right)
    {
        if (left.Unit != right.Unit)
            throw new DomainException("Cannot compare quantities with different units", "MaterialQuantity.Compare.UnitMismatch");
        return left.Value > right.Value;
    }
}
