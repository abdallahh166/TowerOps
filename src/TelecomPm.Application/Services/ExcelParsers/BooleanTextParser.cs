namespace TelecomPm.Application.Services.ExcelParsers;

public static class BooleanTextParser
{
    public static bool? ParseNullable(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var normalized = value.Trim().ToUpperInvariant();

        return normalized switch
        {
            "YES" => true,
            "1" => true,
            "EXIST" => true,
            "NO" => false,
            "0" => false,
            "NOT EXIST" => false,
            "NA" => null,
            _ => null
        };
    }
}
