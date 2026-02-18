namespace TelecomPM.Application.Commands.Sites.UpdateSiteComponents;

using System;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Sites;

public record UpdateSiteComponentsCommand : ICommand<SiteDetailDto>
{
    public Guid SiteId { get; init; }
    public SiteTowerInfoDto? TowerInfo { get; init; }
    public SitePowerSystemDto? PowerSystem { get; init; }
    public SiteRadioEquipmentDto? RadioEquipment { get; init; }
    public SiteTransmissionDto? Transmission { get; init; }
    public SiteCoolingSystemDto? CoolingSystem { get; init; }
    public SiteFireSafetyDto? FireSafety { get; init; }
}

