using System.Globalization;
using System.Text.RegularExpressions;

namespace TelecomPm.Application.Services.ExcelParsers;

public static class BatteryFieldParser
{
    public static BatteryFieldParseResult? Parse(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var normalized = value.Trim();
        if (normalized.Equals("Batteries not installed", StringComparison.OrdinalIgnoreCase))
            return null;

        var compact = Regex.Replace(normalized, "\\s+", " ");

        var voltage = ExtractInteger(compact, "(\\d+)\\s*V");
        var ampereHour = ExtractInteger(compact, "(\\d+)(?:[.,]\\d+)?\\s*AH");

        var brand = ExtractBrand(compact);

        return new BatteryFieldParseResult(brand, voltage, ampereHour);
    }

    private static int? ExtractInteger(string value, string pattern)
    {
        var match = Regex.Match(value, pattern, RegexOptions.IgnoreCase);
        if (!match.Success)
            return null;

        if (int.TryParse(match.Groups[1].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed))
            return parsed;

        return null;
    }

    private static string? ExtractBrand(string value)
    {
        var match = Regex.Match(value, @"^[A-Za-z0-9\-\s]+?(?=\s+\d|\s*\/|$)");
        var brand = match.Success ? match.Value.Trim() : value.Trim();
        return string.IsNullOrWhiteSpace(brand) ? null : brand;
    }
}

public sealed record BatteryFieldParseResult(string? Brand, int? Voltage, int? AmpereHour);
