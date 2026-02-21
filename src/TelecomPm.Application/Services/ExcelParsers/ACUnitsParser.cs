using System.Globalization;
using System.Text.RegularExpressions;

namespace TelecomPm.Application.Services.ExcelParsers;

public static class ACUnitsParser
{
    public static AcUnitsParseResult Parse(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return new AcUnitsParseResult(0, null, null);

        var normalized = value.Trim();
        if (normalized.Equals("Not Exist", StringComparison.OrdinalIgnoreCase))
            return new AcUnitsParseResult(0, null, null);

        if (normalized.Equals("0/Grill", StringComparison.OrdinalIgnoreCase))
            return new AcUnitsParseResult(0, null, null);

        if (int.TryParse(normalized, NumberStyles.Integer, CultureInfo.InvariantCulture, out var countOnly))
            return new AcUnitsParseResult(countOnly, null, null);

        var slashParts = normalized.Split('/', StringSplitOptions.TrimEntries);
        if (slashParts.Length == 2)
        {
            var vendor = string.IsNullOrWhiteSpace(slashParts[0]) ? null : slashParts[0];
            var hp = ExtractHp(slashParts[1]);
            var count = hp.HasValue ? 1 : 0;
            return new AcUnitsParseResult(count, vendor, hp);
        }

        var commaParts = normalized.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        if (commaParts.Length > 1)
        {
            var hpValues = commaParts.Select(ExtractHp).Where(h => h.HasValue).Select(h => h!.Value).ToList();
            decimal? hp = hpValues.Count > 0 ? hpValues[0] : null;
            return new AcUnitsParseResult(commaParts.Length, null, hp);
        }

        return new AcUnitsParseResult(0, null, ExtractHp(normalized));
    }

    private static decimal? ExtractHp(string input)
    {
        var match = Regex.Match(input, @"(\d+(?:[.,]\d+)?)\s*HP", RegexOptions.IgnoreCase);
        if (!match.Success)
            return null;

        var normalized = match.Groups[1].Value.Replace(',', '.');
        if (decimal.TryParse(normalized, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed))
            return parsed;

        return null;
    }
}

public sealed record AcUnitsParseResult(int Count, string? Vendor, decimal? HorsePower);
