using ClosedXML.Excel;
using TelecomPm.Application.Services.ExcelParsers;
using TelecomPM.Domain.Entities.Sites;
using TelecomPM.Domain.Enums;

namespace TelecomPM.Application.Commands.Imports;

internal static class ImportExcelSupport
{
    public static IXLWorksheet? FindWorksheet(XLWorkbook workbook, params string[] candidateNames)
    {
        foreach (var candidate in candidateNames)
        {
            var match = workbook.Worksheets.FirstOrDefault(w =>
                string.Equals(w.Name.Trim(), candidate.Trim(), StringComparison.OrdinalIgnoreCase));

            if (match is not null)
                return match;
        }

        return null;
    }

    public static Dictionary<string, int> BuildColumnMap(IXLRow headerRow)
    {
        var map = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        foreach (var cell in headerRow.CellsUsed())
        {
            var header = cell.GetString().Trim();
            if (!string.IsNullOrWhiteSpace(header) && !map.ContainsKey(header))
                map[header] = cell.Address.ColumnNumber;
        }

        return map;
    }

    public static string GetCellText(IXLRow row, Dictionary<string, int> columnMap, params string[] aliases)
    {
        foreach (var alias in aliases)
        {
            if (columnMap.TryGetValue(alias, out var col))
                return row.Cell(col).GetString().Trim();
        }

        return string.Empty;
    }

    public static object? GetCellRawValue(IXLRow row, Dictionary<string, int> columnMap, params string[] aliases)
    {
        foreach (var alias in aliases)
        {
            if (columnMap.TryGetValue(alias, out var col))
            {
                var cell = row.Cell(col);
                return cell.IsEmpty() ? null : cell.GetString();
            }
        }

        return null;
    }

    public static bool IsBlankOrNa(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ||
               string.Equals(value.Trim(), "NA", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(value.Trim(), "N/A", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(value.Trim(), "-", StringComparison.OrdinalIgnoreCase);
    }

    public static int? ParseInt(string? value)
    {
        if (IsBlankOrNa(value))
            return null;

        var normalized = value!.Trim();

        var firstNumeric = new string(normalized.TakeWhile(c => char.IsDigit(c) || c == '+' || c == '-' || c == '.').ToArray());
        if (int.TryParse(firstNumeric, out var firstParsed))
            return firstParsed;

        if (int.TryParse(normalized, out var parsed))
            return parsed;

        if (double.TryParse(normalized.Replace(',', '.'), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var asDouble))
            return Convert.ToInt32(Math.Truncate(asDouble));

        return null;
    }

    public static decimal? ParseDecimal(string? value)
    {
        if (IsBlankOrNa(value))
            return null;

        var normalized = value!.Trim().Replace(',', '.');
        if (decimal.TryParse(normalized, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var parsed))
            return parsed;

        return null;
    }

    public static DateTime? ParseDateUtc(object? value)
        => ExcelDateParser.ParseToUtc(value);

    public static SiteStatus? ParseSiteStatus(string? value)
    {
        if (IsBlankOrNa(value))
            return null;

        var normalized = value!.Trim().ToUpperInvariant();

        if (normalized.Contains("ON"))
            return SiteStatus.OnAir;

        if (normalized.Contains("OFF"))
            return SiteStatus.OffAir;

        if (normalized.Contains("MAINT"))
            return SiteStatus.UnderMaintenance;

        if (normalized.Contains("DECOM"))
            return SiteStatus.Decommissioned;

        return null;
    }

    public static SiteType ParseSiteTypeOrDefault(string? value, SiteType fallback = SiteType.Macro)
    {
        if (IsBlankOrNa(value))
            return fallback;

        var normalized = value!.Trim().ToUpperInvariant();

        return normalized switch
        {
            "GREEN FIELD" or "GREENFIELD" or "GF" => SiteType.GreenField,
            "ROOF TOP" or "ROOFTOP" or "RT" => SiteType.RoofTop,
            "BTS" => SiteType.BTS,
            "INDOOR" => SiteType.Indoor,
            "OUTDOOR" => SiteType.Outdoor,
            "MACRO" => SiteType.Macro,
            "NODAL" => SiteType.Nodal,
            "BSC" => SiteType.BSC,
            "VIP" => SiteType.VIP,
            "REPEATER" => SiteType.Repeater,
            "MICRONANO" or "MICRO NANO" => SiteType.MicroNano,
            _ => fallback
        };
    }

    public static TransmissionType ParseTransmissionTypeOrDefault(string? value, TransmissionType fallback = TransmissionType.MW)
    {
        if (IsBlankOrNa(value))
            return fallback;

        var normalized = value!.Trim().ToUpperInvariant();
        if (normalized.Contains("FIBER")) return TransmissionType.Fiber;
        if (normalized.Contains("E-BAND") || normalized.Contains("EBAND")) return TransmissionType.EBand;
        if (normalized.Contains("HYBRID")) return TransmissionType.Hybrid;
        return TransmissionType.MW;
    }

    public static PowerConfiguration ParsePowerConfigurationOrDefault(string? value, PowerConfiguration fallback = PowerConfiguration.ACOnly)
    {
        if (IsBlankOrNa(value))
            return fallback;

        var normalized = value!.Trim().ToUpperInvariant();

        if (normalized.Contains("GEN"))
            return PowerConfiguration.Generator;

        if (normalized.Contains("SOLAR") && normalized.Contains("HYBRID"))
            return PowerConfiguration.SolarHybrid;

        if (normalized.Contains("SOLAR"))
            return PowerConfiguration.Solar;

        if (normalized.Contains("DC"))
            return PowerConfiguration.DCOnly;

        if (normalized.Contains("HYBRID"))
            return PowerConfiguration.Hybrid;

        return PowerConfiguration.ACOnly;
    }

    public static SiteEnclosureType? ParseEnclosureType(string? value)
    {
        if (IsBlankOrNa(value))
            return null;

        var normalized = value!.Trim().ToUpperInvariant();

        if (normalized.Contains("SHELTER"))
            return SiteEnclosureType.Shelter;

        if (normalized.Contains("GRILL"))
            return SiteEnclosureType.Grill;

        if (normalized.Contains("OD"))
            return SiteEnclosureType.OutdoorCabinet;

        return null;
    }

    public static Technology ParseTechnologyFromBand(string? value)
    {
        if (IsBlankOrNa(value))
            return Technology.TwoG;

        var normalized = value!.Trim().ToUpperInvariant();

        if (normalized.StartsWith("L"))
            return Technology.FourG;

        if (normalized.StartsWith("U"))
            return Technology.ThreeG;

        return Technology.TwoG;
    }

    public static Dictionary<string, Guid> BuildSiteIdLookup(IEnumerable<Site> sites)
    {
        var lookup = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);

        foreach (var site in sites)
        {
            if (!string.IsNullOrWhiteSpace(site.SiteCode.Value))
                lookup[site.SiteCode.Value.Trim()] = site.Id;

            if (!string.IsNullOrWhiteSpace(site.SiteCode.ShortCode))
                lookup[site.SiteCode.ShortCode.Trim()] = site.Id;
        }

        return lookup;
    }

    public static Dictionary<string, Site> BuildSiteLookup(IEnumerable<Site> sites)
    {
        var lookup = new Dictionary<string, Site>(StringComparer.OrdinalIgnoreCase);

        foreach (var site in sites)
        {
            if (!string.IsNullOrWhiteSpace(site.SiteCode.Value))
                lookup[site.SiteCode.Value.Trim()] = site;

            if (!string.IsNullOrWhiteSpace(site.SiteCode.ShortCode))
                lookup[site.SiteCode.ShortCode.Trim()] = site;
        }

        return lookup;
    }

    public static string NormalizeSiteKey(string? raw)
    {
        return string.IsNullOrWhiteSpace(raw)
            ? string.Empty
            : raw.Trim();
    }

    public static List<string> ParseGuests(string? guests)
    {
        if (IsBlankOrNa(guests))
            return new List<string>();

        return guests!
            .Split(new[] { ',', ';', '/', '|' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(x => !IsBlankOrNa(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }
}
