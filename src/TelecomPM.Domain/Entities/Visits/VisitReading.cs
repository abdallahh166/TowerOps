using TelecomPM.Domain.Common;

namespace TelecomPM.Domain.Entities.Visits;

// ==================== Visit Reading ====================
public sealed class VisitReading : Entity<Guid>
{
    public Guid VisitId { get; private set; }
    public string ReadingType { get; private set; } = string.Empty; // PhaseVoltage, Current, etc.
    public string Category { get; private set; } = string.Empty; // Electrical, Power, Cooling
    public decimal Value { get; private set; }
    public string Unit { get; private set; } = string.Empty;
    public decimal? MinAcceptable { get; private set; }
    public decimal? MaxAcceptable { get; private set; }
    public bool IsWithinRange { get; private set; }
    public DateTime MeasuredAt { get; private set; }
    public string? Phase { get; private set; } // For electrical readings
    public string? Equipment { get; private set; } // Which equipment
    public string? Notes { get; private set; }

    private VisitReading() : base() { }

    private VisitReading(
        Guid visitId,
        string readingType,
        string category,
        decimal value,
        string unit) : base(Guid.NewGuid())
    {
        VisitId = visitId;
        ReadingType = readingType;
        Category = category;
        Value = value;
        Unit = unit;
        MeasuredAt = DateTime.UtcNow;
    }

    public static VisitReading Create(
        Guid visitId,
        string readingType,
        string category,
        decimal value,
        string unit)
    {
        return new VisitReading(visitId, readingType, category, value, unit);
    }

    public void SetValidationRange(decimal min, decimal max)
    {
        MinAcceptable = min;
        MaxAcceptable = max;
        ValidateRange();
    }

    public void SetPhase(string phase)
    {
        Phase = phase;
    }

    public void SetEquipment(string equipment)
    {
        Equipment = equipment;
    }

    public void UpdateValue(decimal newValue)
    {
        Value = newValue;
        MeasuredAt = DateTime.UtcNow;
        ValidateRange();
    }

    public void AddNotes(string notes)
    {
        Notes = notes;
    }

    private void ValidateRange()
    {
        if (MinAcceptable.HasValue && MaxAcceptable.HasValue)
        {
            IsWithinRange = Value >= MinAcceptable.Value && Value <= MaxAcceptable.Value;
        }
        else
        {
            IsWithinRange = true;
        }
    }
}
