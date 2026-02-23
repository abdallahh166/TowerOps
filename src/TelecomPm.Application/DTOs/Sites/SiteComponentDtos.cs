namespace TelecomPM.Application.DTOs.Sites;

using System.Collections.Generic;
using TelecomPM.Domain.Enums;

public record CoordinatesDto(double Latitude, double Longitude);

public record AddressDto(string Street, string City, string Region, string Details);

public record SiteTowerInfoDto
{
    public TowerType Type { get; init; }
    public int Height { get; init; }
    public string Owner { get; init; } = string.Empty;
    public int NumberOfMasts { get; init; }
    public bool NeedsRepair { get; init; }
}

public record SitePowerSystemDto
{
    public PowerConfiguration Configuration { get; init; }
    public RectifierBrand RectifierBrand { get; init; }
    public int RectifierModulesCount { get; init; }
    public BatteryType BatteryType { get; init; }
    public int BatteryStrings { get; init; }
    public bool HasGenerator { get; init; }
    public bool HasSolarPanel { get; init; }
    public string? GeneratorType { get; init; }
    public int? GeneratorKVA { get; init; }
    public string? CabinetType { get; init; }
    public decimal? ChargingCurrentLimit { get; init; }
}

public record SiteRadioEquipmentDto
{
    public bool Has2G { get; init; }
    public bool Has3G { get; init; }
    public bool Has4G { get; init; }
    public BTSVendor? BTSVendor { get; init; }
    public BTSVendor? NodeBVendor { get; init; }
    public int SectorsCount { get; init; }
}

public record SiteTransmissionDto
{
    public TransmissionType Type { get; init; }
    public string Supplier { get; init; } = string.Empty;
    public int LinksCount { get; init; }
    public bool HasGPS { get; init; }
    public bool HasEBand { get; init; }
}

public record SiteCoolingSystemDto
{
    public int ACUnitsCount { get; init; }
    public bool HasVentilation { get; init; }
    public bool HasDCACUnit { get; init; }
}

public record SiteFireSafetyDto
{
    public string FirePanelType { get; init; } = string.Empty;
    public int HeatSensorsCount { get; init; }
    public int SmokeSensorsCount { get; init; }
    public int ExtinguishersCount { get; init; }
}

public record SiteSharingDto
{
    public bool IsShared { get; init; }
    public string? HostOperator { get; init; }
    public List<string> GuestOperators { get; init; } = new();
    public bool PowerShared { get; init; }
    public bool TowerShared { get; init; }
}
