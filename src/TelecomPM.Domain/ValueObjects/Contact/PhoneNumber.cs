using System.Collections.Generic;
using System.Text.RegularExpressions;
using TelecomPM.Domain.Exceptions;

namespace TelecomPM.Domain.ValueObjects;

// ==================== Phone Number ====================
public sealed class PhoneNumber : ValueObject
{
    public string Value { get; }

    private PhoneNumber(string value) => Value = value;

    public static PhoneNumber Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Phone number is required", "PhoneNumber.Required");

        // Validate format - Egyptian phone format: +20 10/11/12/15 XXXXXXXX
        if (!IsValidPhoneNumber(value))
            throw new DomainException("Invalid phone number format. Expected format: +20 10/11/12/15 XXXXXXXX", "PhoneNumber.InvalidFormat");

        return new PhoneNumber(value);
    }

    private static bool IsValidPhoneNumber(string phone)
    {
        // Remove spaces for validation
        var normalized = phone.Replace(" ", "").Replace("-", "");
        
        // Egyptian phone format: +20 10/11/12/15 XXXXXXXX
        return Regex.IsMatch(
            normalized, 
            @"^\+20(10|11|12|15)\d{8}$");
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}

