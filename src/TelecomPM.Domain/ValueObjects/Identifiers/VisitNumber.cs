using System.Collections.Generic;
using System.Text.RegularExpressions;
using TelecomPM.Domain.Exceptions;

namespace TelecomPM.Domain.ValueObjects;

// ==================== Visit Number ====================
public sealed class VisitNumber : ValueObject
{
    public string Value { get; }
    public string Prefix { get; } // V2025
    public int SequenceNumber { get; }

    private VisitNumber(string value, string prefix, int sequenceNumber)
    {
        Value = value;
        Prefix = prefix;
        SequenceNumber = sequenceNumber;
    }

    public static VisitNumber Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Visit number cannot be empty");

        // Format: V2025001, V2025123, etc.
        // Pattern: V + YYYY + 3-6 digits
        var match = Regex.Match(value, @"^V(\d{4})(\d{3,6})$");
        if (!match.Success)
            throw new DomainException("Visit number must be in format VYYYYNNN (e.g., V2025001)");

        var prefix = $"V{match.Groups[1].Value}";
        if (!int.TryParse(match.Groups[2].Value, out int sequenceNumber))
            throw new DomainException("Visit number sequence must be numeric");

        if (sequenceNumber < 1)
            throw new DomainException("Visit number sequence must be greater than zero");

        return new VisitNumber(value.ToUpper(), prefix, sequenceNumber);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}

