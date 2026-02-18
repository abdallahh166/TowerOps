namespace TelecomPM.Application.DTOs.Sites;

public record SiteDetailDto : SiteDto
{
    public CoordinatesDto Coordinates { get; init; } = null!;
    public AddressDto Address { get; init; } = null!;
    public SiteTowerInfoDto? TowerInfo { get; init; }
    public SitePowerSystemDto? PowerSystem { get; init; }
    public SiteRadioEquipmentDto? RadioEquipment { get; init; }
    public SiteTransmissionDto? Transmission { get; init; }
    public SiteCoolingSystemDto? CoolingSystem { get; init; }
    public SiteFireSafetyDto? FireSafety { get; init; }
    public SiteSharingDto? SharingInfo { get; init; }
}