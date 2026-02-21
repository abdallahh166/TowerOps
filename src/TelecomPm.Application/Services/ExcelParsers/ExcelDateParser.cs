using System.Globalization;

namespace TelecomPm.Application.Services.ExcelParsers;

public static class ExcelDateParser
{
    private static readonly string[] SupportedDateFormats =
    [
        "dd/MM/yyyy",
        "d/M/yyyy",
        "MM/dd/yyyy",
        "M/d/yyyy",
        "yyyy-MM-dd"
    ];

    public static DateTime? ParseToUtc(object? value)
    {
        if (value is null)
            return null;

        if (value is DateTime dateTime)
            return EnsureUtc(dateTime);

        if (value is int intSerial)
            return FromExcelSerial(intSerial);

        if (value is double doubleSerial)
            return FromExcelSerial(doubleSerial);

        var text = value.ToString()?.Trim();
        if (string.IsNullOrWhiteSpace(text))
            return null;

        if (double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsedSerial))
            return FromExcelSerial(parsedSerial);

        if (DateTime.TryParseExact(text, SupportedDateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var exactDate))
            return EnsureUtc(exactDate);

        if (DateTime.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
            return EnsureUtc(parsedDate);

        return null;
    }

    private static DateTime FromExcelSerial(double serial)
    {
        var wholeDays = (int)Math.Floor(serial);
        var fraction = serial - wholeDays;

        var baseDate = new DateTime(1899, 12, 31, 0, 0, 0, DateTimeKind.Utc);

        // Excel keeps the fake 1900-02-29 (Lotus 1-2-3 compatibility bug).
        if (wholeDays > 59)
            wholeDays -= 1;

        return baseDate.AddDays(wholeDays).AddDays(fraction);
    }

    private static DateTime EnsureUtc(DateTime value)
    {
        return value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
        };
    }
}
