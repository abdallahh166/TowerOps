using System.Collections.Generic;
using System.Text.RegularExpressions;
using TowerOps.Domain.Exceptions;

namespace TowerOps.Domain.ValueObjects;

// ==================== Material Code ====================
public sealed class MaterialCode : ValueObject
{
    public string Value { get; }

    private MaterialCode(string value) => Value = value;

    public static MaterialCode Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Material code cannot be empty", "MaterialCode.Required");

        // Material codes should be alphanumeric, typically 3-20 characters
        if (value.Length < 3 || value.Length > 20)
            throw new DomainException("Material code must be between 3 and 20 characters", "MaterialCode.LengthRange");

        // Allow alphanumeric and common separators (hyphen, underscore)
        if (!Regex.IsMatch(value, @"^[A-Z0-9][A-Z0-9\-_]*$", RegexOptions.IgnoreCase))
            throw new DomainException("Material code can only contain letters, numbers, hyphens, and underscores", "MaterialCode.InvalidFormat");

        return new MaterialCode(value.ToUpper());
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}

