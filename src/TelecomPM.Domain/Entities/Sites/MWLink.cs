namespace TelecomPM.Domain.Entities.Sites;

// ==================== MW Link ====================
public sealed class MWLink
{
    public string LinkDirection { get; private set; } = string.Empty;
    public string OppositeSite { get; private set; } = string.Empty;
    public string FrequencyBand { get; private set; } = string.Empty;
    public int DishSizeCM { get; private set; }
    public string ODUType { get; private set; } = string.Empty;
    public string? ManagementIpAddress { get; private set; }
    public decimal? TxFrequencyKHz { get; private set; }
    public decimal? RxFrequencyKHz { get; private set; }
    public decimal? TxPowerDbm { get; private set; }
    public decimal? RxPowerDbm { get; private set; }
    public decimal? CapacityMbps { get; private set; }
    public string? Modulation { get; private set; }
    public string? Configuration { get; private set; }
    public string? Polarization { get; private set; }
    public string? OduSerialNumber { get; private set; }
    public string? OppositeOduSerialNumber { get; private set; }
    public string? AntennaReference { get; private set; }
    public decimal? TxAzimuth { get; private set; }
    public decimal? TxHbaMeters { get; private set; }

    private MWLink() { }

    public static MWLink Create(
        string linkDirection,
        string oppositeSite,
        string frequencyBand,
        int dishSize,
        string oduType)
    {
        return new MWLink
        {
            LinkDirection = linkDirection,
            OppositeSite = oppositeSite,
            FrequencyBand = frequencyBand,
            DishSizeCM = dishSize,
            ODUType = oduType
        };
    }

    public void SetRadioDetails(
        decimal? txFrequencyKHz,
        decimal? rxFrequencyKHz,
        decimal? txPowerDbm,
        decimal? rxPowerDbm,
        decimal? capacityMbps,
        string? modulation,
        string? configuration,
        string? polarization)
    {
        TxFrequencyKHz = txFrequencyKHz;
        RxFrequencyKHz = rxFrequencyKHz;
        TxPowerDbm = txPowerDbm;
        RxPowerDbm = rxPowerDbm;
        CapacityMbps = capacityMbps;
        Modulation = modulation;
        Configuration = configuration;
        Polarization = polarization;
    }

    public void SetAssetDetails(
        string? managementIpAddress,
        string? oduSerialNumber,
        string? oppositeOduSerialNumber,
        string? antennaReference,
        decimal? txAzimuth,
        decimal? txHbaMeters)
    {
        ManagementIpAddress = managementIpAddress;
        OduSerialNumber = oduSerialNumber;
        OppositeOduSerialNumber = oppositeOduSerialNumber;
        AntennaReference = antennaReference;
        TxAzimuth = txAzimuth;
        TxHbaMeters = txHbaMeters;
    }
}
