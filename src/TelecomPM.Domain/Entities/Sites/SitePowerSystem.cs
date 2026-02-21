using TelecomPM.Domain.Common;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Exceptions;

namespace TelecomPM.Domain.Entities.Sites;

// ==================== Site Power System ====================
public sealed class SitePowerSystem : Entity<Guid>
{
    public Guid SiteId { get; private set; }
    public PowerConfiguration Configuration { get; private set; }
    
    // Rectifier
    public RectifierBrand RectifierBrand { get; private set; }
    public int RectifierModulesCount { get; private set; }
    public string? RectifierControllerType { get; private set; }
    
    // Batteries
    public BatteryType BatteryType { get; private set; }
    public int BatteryStrings { get; private set; }
    public int BatteriesPerString { get; private set; }
    public int BatteryAmpereHour { get; private set; }
    public int BatteryVoltage { get; private set; }
    public string? BatteryBrand { get; private set; }
    public string? BatteryHealthStatus { get; private set; }
    public bool? IsCabinetized { get; private set; }
    public string? CabinetVendor { get; private set; }
    
    // Solar (if applicable)
    public bool HasSolarPanel { get; private set; }
    public int? SolarPanelWatt { get; private set; }
    public int? SolarPanelsCount { get; private set; }
    
    // Generator (if applicable)
    public bool HasGenerator { get; private set; }
    public string? GeneratorType { get; private set; }
    public string? GeneratorSerialNumber { get; private set; }
    public int? GeneratorKVA { get; private set; }
    public int? FuelTankSizeLiters { get; private set; }
    
    // Power Meter
    public bool HasPowerMeter { get; private set; }
    public int? PowerMeterRate { get; private set; }
    public string? ElectricityPhaseType { get; private set; }
    public int? RouterCount { get; private set; }
    public int? ModemCount { get; private set; }

    private SitePowerSystem() : base() { }

    private SitePowerSystem(
        Guid siteId,
        PowerConfiguration configuration,
        RectifierBrand rectifierBrand,
        BatteryType batteryType) : base(Guid.NewGuid())
    {
        SiteId = siteId;
        Configuration = configuration;
        RectifierBrand = rectifierBrand;
        BatteryType = batteryType;
    }

    public static SitePowerSystem Create(
        Guid siteId,
        PowerConfiguration configuration,
        RectifierBrand rectifierBrand,
        BatteryType batteryType)
    {
        return new SitePowerSystem(siteId, configuration, rectifierBrand, batteryType);
    }

    public void SetRectifierDetails(int modulesCount, string? controllerType = null)
    {
        if (modulesCount <= 0)
            throw new DomainException("Rectifier modules count must be greater than zero");

        RectifierModulesCount = modulesCount;
        RectifierControllerType = controllerType;
    }

    public void SetBatteryDetails(int strings, int batteriesPerString, int ampereHour, int voltage)
    {
        if (strings <= 0)
            throw new DomainException("Battery strings must be greater than zero");

        BatteryStrings = strings;
        BatteriesPerString = batteriesPerString;
        BatteryAmpereHour = ampereHour;
        BatteryVoltage = voltage;
    }

    public void SetBatteryMetadata(string? batteryBrand, string? batteryHealthStatus)
    {
        BatteryBrand = batteryBrand;
        BatteryHealthStatus = batteryHealthStatus;
    }

    public void SetCabinetInfo(bool? isCabinetized, string? cabinetVendor)
    {
        IsCabinetized = isCabinetized;
        CabinetVendor = cabinetVendor;
    }

    public void SetSolarPanel(int panelWatt, int panelsCount)
    {
        HasSolarPanel = true;
        SolarPanelWatt = panelWatt;
        SolarPanelsCount = panelsCount;
    }

    public void SetGenerator(string type, string serialNumber, int kva, int fuelTankSize)
    {
        HasGenerator = true;
        GeneratorType = type;
        GeneratorSerialNumber = serialNumber;
        GeneratorKVA = kva;
        FuelTankSizeLiters = fuelTankSize;
    }

    public void SetPowerMeter(int rate, string phaseType)
    {
        HasPowerMeter = true;
        PowerMeterRate = rate;
        ElectricityPhaseType = phaseType;
    }

    public void SetNetworkEquipmentCounts(int? routerCount, int? modemCount)
    {
        RouterCount = routerCount;
        ModemCount = modemCount;
    }
}
