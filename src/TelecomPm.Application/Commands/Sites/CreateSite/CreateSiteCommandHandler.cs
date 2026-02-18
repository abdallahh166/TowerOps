namespace TelecomPM.Application.Commands.Sites.CreateSite;

using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Sites;
using TelecomPM.Domain.Entities.Sites;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Interfaces.Repositories;
using TelecomPM.Domain.ValueObjects;

public class CreateSiteCommandHandler : IRequestHandler<CreateSiteCommand, Result<SiteDetailDto>>
{
    private readonly ISiteRepository _siteRepository;
    private readonly IOfficeRepository _officeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateSiteCommandHandler(
        ISiteRepository siteRepository,
        IOfficeRepository officeRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _siteRepository = siteRepository;
        _officeRepository = officeRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<SiteDetailDto>> Handle(CreateSiteCommand request, CancellationToken cancellationToken)
    {
        // Validate office exists
        var office = await _officeRepository.GetByIdAsync(request.OfficeId, cancellationToken);
        if (office == null)
            return Result.Failure<SiteDetailDto>("Office not found");

        // Check if site code already exists
        var existingSite = await _siteRepository.GetBySiteCodeAsync(request.SiteCode, cancellationToken);
        if (existingSite != null)
            return Result.Failure<SiteDetailDto>("Site code already exists");

        try
        {
            var coordinates = Coordinates.Create(request.Latitude, request.Longitude);
            var address = Address.Create(request.Street, request.City, request.AddressRegion, request.AddressDetails);

            var site = Site.Create(
                request.SiteCode,
                request.Name,
                request.OMCName,
                request.OfficeId,
                request.Region,
                request.SubRegion,
                coordinates,
                address,
                request.SiteType);

            if (!string.IsNullOrWhiteSpace(request.BSCName) && !string.IsNullOrWhiteSpace(request.BSCCode))
            {
                site.SetBSCInfo(request.BSCName, request.BSCCode);
            }

            if (!string.IsNullOrWhiteSpace(request.Subcontractor))
            {
                site.SetContractorInfo(
                    request.Subcontractor, 
                    request.MaintenanceArea ?? string.Empty);
            }

            // Initialize default components (required by entity)
            var defaultTower = SiteTowerInfo.Create(site.Id, TowerType.GFTower, 30, "TEData");
            site.SetTowerInfo(defaultTower);

            var defaultPower = SitePowerSystem.Create(site.Id, PowerConfiguration.ACOnly, RectifierBrand.Delta, BatteryType.VRLA);
            site.SetPowerSystem(defaultPower);

            var defaultRadio = SiteRadioEquipment.Create(site.Id);
            site.SetRadioEquipment(defaultRadio);

            var defaultTransmission = SiteTransmission.Create(site.Id, TransmissionType.MW, "Default");
            site.SetTransmission(defaultTransmission);

            var defaultCooling = SiteCoolingSystem.Create(site.Id, 1); // Minimum 1 AC unit
            site.SetCoolingSystem(defaultCooling);

            var defaultFireSafety = SiteFireSafety.Create(site.Id, "Default");
            site.SetFireSafety(defaultFireSafety);

            await _siteRepository.AddAsync(site, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var dto = _mapper.Map<SiteDetailDto>(site);
            return Result.Success(dto);
        }
        catch (System.Exception ex)
        {
            return Result.Failure<SiteDetailDto>($"Failed to create site: {ex.Message}");
        }
    }
}

