using System.Collections.Generic;
using TelecomPM.Domain.Exceptions;

namespace TelecomPM.Domain.ValueObjects;

// ==================== Site Code ====================
public sealed class SiteCode : ValueObject
{
    public string Value { get; }
    public string OfficeCode { get; }
    public int SequenceNumber { get; }

    private SiteCode(string value, string officeCode, int sequenceNumber)
    {
        Value = value;
        OfficeCode = officeCode;
        SequenceNumber = sequenceNumber;
    }

    public static SiteCode Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Site code cannot be empty");

        // Format: TNT001, ALX123, etc.
        if (value.Length < 4)
            throw new DomainException("Site code must be at least 4 characters");

        var officeCode = value.Substring(0, 3).ToUpper();
        if (!int.TryParse(value.Substring(3), out int sequenceNumber))
            throw new DomainException("Site code must end with numbers");

        return new SiteCode(value.ToUpper(), officeCode, sequenceNumber);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
