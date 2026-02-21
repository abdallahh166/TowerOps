using System.Globalization;

namespace TelecomPm.Application.Services.ExcelParsers;

public static class CoordinateSplitter
{
    public static (decimal Latitude, decimal Longitude)? Parse(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var parts = value.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 2)
            return null;

        if (!decimal.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out var latitude))
            return null;

        if (!decimal.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out var longitude))
            return null;

        if (latitude < -90 || latitude > 90)
            return null;

        if (longitude < -180 || longitude > 180)
            return null;

        return (latitude, longitude);
    }
}
