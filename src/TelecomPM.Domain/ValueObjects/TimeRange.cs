using System.Collections.Generic;
using TelecomPM.Domain.Exceptions;

namespace TelecomPM.Domain.ValueObjects;

// ==================== Time Range ====================
public sealed class TimeRange : ValueObject
{
    public DateTime Start { get; }
    public DateTime End { get; }
    public TimeSpan Duration => End - Start;

    private TimeRange(DateTime start, DateTime end)
    {
        Start = start;
        End = end;
    }

    public static TimeRange Create(DateTime start, DateTime end)
    {
        if (end <= start)
            throw new DomainException("End time must be after start time", "TimeRange.EndAfterStartRequired");

        return new TimeRange(start, end);
    }

    public bool IsValid()
    {
        var minDuration = TimeSpan.FromMinutes(30);
        var maxDuration = TimeSpan.FromHours(8);

        return Duration >= minDuration && Duration <= maxDuration;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Start;
        yield return End;
    }

    public override string ToString() => $"{Start:HH:mm} - {End:HH:mm} ({Duration.TotalHours:F1}h)";
}
