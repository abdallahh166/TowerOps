using System.Collections.Generic;
using TelecomPM.Domain.Exceptions;

namespace TelecomPM.Domain.ValueObjects;

// ==================== Current Reading ====================
public sealed class CurrentReading : ValueObject
{
    public decimal Value { get; }
    public string Phase { get; }
    public DateTime MeasuredAt { get; }
    public bool IsWithinRange { get; }

    private CurrentReading(decimal value, string phase, DateTime measuredAt, bool isWithinRange)
    {
        Value = value;
        Phase = phase;
        MeasuredAt = measuredAt;
        IsWithinRange = isWithinRange;
    }

    public static CurrentReading CreatePhase(decimal value, string phase, DateTime? measuredAt = null)
    {
        const decimal MAX = 100m;
        var isValid = value <= MAX;

        return new CurrentReading(value, phase, measuredAt ?? DateTime.UtcNow, isValid);
    }

    public static CurrentReading CreateNeutral(decimal value, DateTime? measuredAt = null)
    {
        const decimal MAX = 30m;
        var isValid = value <= MAX;

        return new CurrentReading(value, "N", measuredAt ?? DateTime.UtcNow, isValid);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
        yield return Phase;
        yield return MeasuredAt;
    }

    public override string ToString() => $"{Value}A ({Phase})";
}
