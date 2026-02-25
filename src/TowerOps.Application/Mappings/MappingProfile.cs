namespace TowerOps.Application.Mappings;

using AutoMapper;
using TowerOps.Application.DTOs.Escalations;
using TowerOps.Application.DTOs.Materials;
using TowerOps.Application.DTOs.Offices;
using TowerOps.Application.DTOs.Sites;
using TowerOps.Application.DTOs.Users;
using TowerOps.Application.DTOs.Visits;
using TowerOps.Domain.Entities.Escalations;
using TowerOps.Domain.Entities.Materials;
using TowerOps.Domain.Entities.Offices;
using TowerOps.Domain.Entities.Sites;
using TowerOps.Domain.Entities.Users;
using TowerOps.Domain.Entities.Visits;
using TowerOps.Domain.Entities.WorkOrders;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateVisitMappings();
        CreateSiteMappings();
        CreateMaterialMappings();
        CreateUserMappings();
        CreateOfficeMappings();
        CreateWorkOrderMappings();
    }

    private void CreateVisitMappings()
    {
        CreateMap<Visit, VisitDto>()
            .ForMember(d => d.Duration, opt => opt.MapFrom(s =>
                s.ActualDuration != null ? s.ActualDuration.ToString() : null))
            .ForMember(d => d.CanBeEdited, opt => opt.MapFrom(s => s.CanBeEdited()))
            .ForMember(d => d.CanBeSubmitted, opt => opt.MapFrom(s => s.CanBeSubmitted()));

        CreateMap<Visit, VisitDetailDto>()
            .IncludeBase<Visit, VisitDto>();

        CreateMap<VisitPhoto, VisitPhotoDto>()
            .ForMember(d => d.FileUrl, opt => opt.MapFrom(s => s.FilePath))
            .ForMember(d => d.ThumbnailUrl, opt => opt.MapFrom(s => s.ThumbnailPath))
            .ForMember(d => d.CapturedAt, opt => opt.MapFrom(s => s.CapturedAtUtc ?? s.CreatedAt));

        CreateMap<VisitReading, VisitReadingDto>();
        CreateMap<VisitChecklist, VisitChecklistDto>();

        CreateMap<VisitMaterialUsage, VisitMaterialUsageDto>()
            .ForMember(d => d.Quantity, opt => opt.MapFrom(s => s.Quantity.Value))
            .ForMember(d => d.Unit, opt => opt.MapFrom(s => s.Quantity.Unit.ToString()))
            .ForMember(d => d.UnitCost, opt => opt.MapFrom(s => s.UnitCost.Amount))
            .ForMember(d => d.TotalCost, opt => opt.MapFrom(s => s.TotalCost.Amount));

        CreateMap<VisitIssue, VisitIssueDto>()
            .ForMember(d => d.PhotoUrls, opt => opt.Ignore());

        CreateMap<VisitApproval, VisitApprovalDto>();
    }

    private void CreateSiteMappings()
    {
        CreateMap<Site, SiteDto>()
            .ForMember(d => d.SiteCode, opt => opt.MapFrom(s => s.SiteCode.Value));

        CreateMap<Site, SiteDetailDto>()
            .IncludeBase<Site, SiteDto>()
            .ForMember(d => d.Coordinates, opt => opt.MapFrom(s =>
                new CoordinatesDto(s.Coordinates.Latitude, s.Coordinates.Longitude)))
            .ForMember(d => d.Address, opt => opt.MapFrom(s =>
                new AddressDto(s.Address.Street, s.Address.City, s.Address.Region, s.Address.Details)));

        CreateMap<SiteTowerInfo, SiteTowerInfoDto>();
        CreateMap<SitePowerSystem, SitePowerSystemDto>();
        CreateMap<SiteRadioEquipment, SiteRadioEquipmentDto>();
        CreateMap<SiteTransmission, SiteTransmissionDto>();
        CreateMap<SiteCoolingSystem, SiteCoolingSystemDto>();

        CreateMap<SiteFireSafety, SiteFireSafetyDto>()
            .ForMember(d => d.ExtinguishersCount, opt => opt.MapFrom(s => s.Extinguishers.Count));

        CreateMap<SiteSharing, SiteSharingDto>();
    }

    private void CreateMaterialMappings()
    {
        CreateMap<Material, MaterialDto>()
            .ForMember(d => d.CurrentStock, opt => opt.MapFrom(s => s.CurrentStock.Value))
            .ForMember(d => d.Unit, opt => opt.MapFrom(s => s.CurrentStock.Unit.ToString()))
            .ForMember(d => d.MinimumStock, opt => opt.MapFrom(s => s.MinimumStock.Value))
            .ForMember(d => d.UnitCost, opt => opt.MapFrom(s => s.UnitCost.Amount))
            .ForMember(d => d.Currency, opt => opt.MapFrom(s => s.UnitCost.Currency))
            .ForMember(d => d.IsLowStock, opt => opt.MapFrom(s => s.IsStockLow()));
    }

    private void CreateUserMappings()
    {
        CreateMap<User, UserDto>()
            .ForMember(d => d.OfficeName, opt => opt.Ignore())
            .ForMember(d => d.AssignedSitesCount, opt => opt.MapFrom(s => s.AssignedSiteIds.Count));
    }

    private void CreateOfficeMappings()
    {
        CreateMap<Office, OfficeDto>()
            .ForMember(d => d.City, opt => opt.MapFrom(s => s.Address.City));
    }

    private void CreateWorkOrderMappings()
    {
        CreateMap<WorkOrder, TowerOps.Application.DTOs.WorkOrders.WorkOrderDto>();
    }
}
