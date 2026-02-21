using System.Collections.Generic;
using TelecomPM.Domain.Exceptions;

namespace TelecomPM.Domain.ValueObjects;

// ==================== Site Code ====================
public sealed class SiteCode : ValueObject
{
    public string Value { get; }
    public string OfficeCode { get; }
    public int SequenceNumber { get; }
    public string? ShortCode { get; }

    private SiteCode(string value, string officeCode, int sequenceNumber, string? shortCode)
    {
        Value = value;
        OfficeCode = officeCode;
        SequenceNumber = sequenceNumber;
        ShortCode = shortCode;
    }

    public static SiteCode Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Site code cannot be empty");

        var normalized = value.Trim().ToUpperInvariant();

        if (TryParseLegacyFormat(normalized, out var officeCode, out var sequenceNumber))
        {
            var shortCode = $"{sequenceNumber}{officeCode}";
            return new SiteCode(normalized, officeCode, sequenceNumber, shortCode);
        }

        if (TryParseShortCodeFormat(normalized, out officeCode, out sequenceNumber))
        {
            return new SiteCode(normalized, officeCode, sequenceNumber, normalized);
        }

        throw new DomainException("Site code format is invalid");
    }

    public static SiteCode FromShortCode(string shortCode)
    {
        if (string.IsNullOrWhiteSpace(shortCode))
            throw new DomainException("Short code cannot be empty");

        var normalized = shortCode.Trim().ToUpperInvariant();
        if (!TryParseShortCodeFormat(normalized, out var officeCode, out var sequenceNumber))
            throw new DomainException("Short code must follow pattern digits + office letters (for example: 3564DE)");

        return new SiteCode(normalized, officeCode, sequenceNumber, normalized);
    }

    private static bool TryParseLegacyFormat(string value, out string officeCode, out int sequenceNumber)
    {
        officeCode = string.Empty;
        sequenceNumber = 0;

        if (value.Length < 4)
            return false;

        var officePart = value.Substring(0, 3);
        if (!officePart.All(char.IsLetter))
            return false;

        var sequencePart = value.Substring(3);
        if (!int.TryParse(sequencePart, out sequenceNumber))
            return false;

        officeCode = officePart;
        return true;
    }

    private static bool TryParseShortCodeFormat(string value, out string officeCode, out int sequenceNumber)
    {
        officeCode = string.Empty;
        sequenceNumber = 0;

        var splitIndex = 0;
        while (splitIndex < value.Length && char.IsDigit(value[splitIndex]))
        {
            splitIndex++;
        }

        if (splitIndex == 0 || splitIndex >= value.Length)
            return false;

        var numericPart = value[..splitIndex];
        var officePart = value[splitIndex..];

        if (!officePart.All(char.IsLetter))
            return false;

        if (!int.TryParse(numericPart, out sequenceNumber))
            return false;

        officeCode = officePart.ToUpperInvariant();
        return true;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
