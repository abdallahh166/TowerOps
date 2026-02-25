namespace TowerOps.Application.Commands.Sites.UpdateSiteComponents;

using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Sites;
using TowerOps.Domain.Entities.Sites;
using TowerOps.Domain.Enums;
using TowerOps.Domain.Interfaces.Repositories;

public class UpdateSiteComponentsCommandHandler : IRequestHandler<UpdateSiteComponentsCommand, Result<SiteDetailDto>>
{
    private readonly ISiteRepository _siteRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateSiteComponentsCommandHandler(
        ISiteRepository siteRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _siteRepository = siteRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<SiteDetailDto>> Handle(UpdateSiteComponentsCommand request, CancellationToken cancellationToken)
    {
        var site = await _siteRepository.GetByIdAsync(request.SiteId, cancellationToken);
        if (site == null)
            return Result.Failure<SiteDetailDto>("Site not found");

        try
        {
            // Update components only if provided
            if (request.TowerInfo != null)
            {
                var tower = SiteTowerInfo.Create(
                    site.Id,
                    request.TowerInfo.Type,
                    request.TowerInfo.Height,
                    request.TowerInfo.Owner);
                tower.UpdateStructure(request.TowerInfo.NumberOfMasts, 4); // Default wires per mast
                if (request.TowerInfo.NeedsRepair)
                    tower.MarkForRepair(true, false);
                site.SetTowerInfo(tower);
            }

            if (request.PowerSystem != null)
            {
                var power = SitePowerSystem.Create(
                    site.Id,
                    request.PowerSystem.Configuration,
                    request.PowerSystem.RectifierBrand,
                    request.PowerSystem.BatteryType);
                if (request.PowerSystem.RectifierModulesCount > 0)
                    power.SetRectifierDetails(request.PowerSystem.RectifierModulesCount);
                if (request.PowerSystem.BatteryStrings > 0)
                    power.SetBatteryDetails(request.PowerSystem.BatteryStrings, 1, 100, 48); // Defaults
                if (request.PowerSystem.HasGenerator && request.PowerSystem.GeneratorKVA.HasValue)
                    power.SetGenerator(request.PowerSystem.GeneratorType ?? "Diesel", "GEN001", request.PowerSystem.GeneratorKVA.Value, 300);
                if (request.PowerSystem.HasSolarPanel)
                    power.SetSolarPanel(3000, 10); // Defaults
                site.SetPowerSystem(power);
            }

            if (request.RadioEquipment != null)
            {
                var radio = SiteRadioEquipment.Create(site.Id);
                if (request.RadioEquipment.Has2G && request.RadioEquipment.BTSVendor.HasValue)
                    radio.Enable2G(request.RadioEquipment.BTSVendor.Value, "BTS", 1, 2);
                if (request.RadioEquipment.Has3G && request.RadioEquipment.NodeBVendor.HasValue)
                    radio.Enable3G(request.RadioEquipment.NodeBVendor.Value, "NodeB", 2, 1);
                if (request.RadioEquipment.Has4G)
                    radio.Enable4G(request.RadioEquipment.SectorsCount);
                site.SetRadioEquipment(radio);
            }

            if (request.Transmission != null)
            {
                var transmission = SiteTransmission.Create(site.Id, request.Transmission.Type, request.Transmission.Supplier);
                transmission.SetEquipment(request.Transmission.HasGPS, false, false, request.Transmission.HasEBand, false);
                site.SetTransmission(transmission);
            }

            if (request.CoolingSystem != null)
            {
                var cooling = SiteCoolingSystem.Create(site.Id, request.CoolingSystem.ACUnitsCount);
                if (request.CoolingSystem.HasVentilation)
                    cooling.SetVentilation(true);
                if (request.CoolingSystem.HasDCACUnit)
                    cooling.SetDCACUnit(true);
                site.SetCoolingSystem(cooling);
            }

            if (request.FireSafety != null)
            {
                var fireSafety = SiteFireSafety.Create(site.Id, request.FireSafety.FirePanelType);
                fireSafety.SetSensors(
                    request.FireSafety.HeatSensorsCount,
                    request.FireSafety.SmokeSensorsCount,
                    0); // Flame sensors default
                site.SetFireSafety(fireSafety);
            }

            await _siteRepository.UpdateAsync(site, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var dto = _mapper.Map<SiteDetailDto>(site);
            return Result.Success(dto);
        }
        catch (System.Exception ex)
        {
            return Result.Failure<SiteDetailDto>($"Failed to update site components: {ex.Message}");
        }
    }
}

