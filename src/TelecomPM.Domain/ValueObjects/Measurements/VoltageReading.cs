using System.Collections.Generic;
using TelecomPM.Domain.Exceptions;

namespace TelecomPM.Domain.ValueObjects;

// ==================== Voltage Reading ====================
public sealed class VoltageReading : ValueObject
{
    public decimal Value { get; }
    public string Unit { get; }
    public DateTime MeasuredAt { get; }
    public bool IsWithinRange { get; }

    private VoltageReading(decimal value, string unit, DateTime measuredAt, bool isWithinRange)
    {
        Value = value;
        Unit = unit;
        MeasuredAt = measuredAt;
        IsWithinRange = isWithinRange;
    }

    public static VoltageReading CreatePhaseToNeutral(decimal value, DateTime? measuredAt = null)
    {
        const decimal MIN = 180m;
        const decimal MAX = 240m;
        var isValid = value >= MIN && value <= MAX;

        return new VoltageReading(value, "V", measuredAt ?? DateTime.UtcNow, isValid);
    }

    public static VoltageReading CreatePhaseToPhase(decimal value, DateTime? measuredAt = null)
    {
        const decimal MIN = 310m;
        const decimal MAX = 415m;
        var isValid = value >= MIN && value <= MAX;

        return new VoltageReading(value, "V", measuredAt ?? DateTime.UtcNow, isValid);
    }

    public static VoltageReading CreateNeutralToEarth(decimal value, DateTime? measuredAt = null)
    {
        const decimal MAX = 1m;
        var isValid = value <= MAX;

        return new VoltageReading(value, "V", measuredAt ?? DateTime.UtcNow, isValid);
    }

    public static VoltageReading CreateDC(decimal value, DateTime? measuredAt = null)
    {
        const decimal MIN = -56m;
        const decimal MAX = -52m;
        var isValid = value >= MIN && value <= MAX;

        return new VoltageReading(value, "Vdc", measuredAt ?? DateTime.UtcNow, isValid);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
        yield return Unit;
        yield return MeasuredAt;
    }

    public override string ToString() => $"{Value} {Unit}";
}
