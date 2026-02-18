namespace TelecomPM.Application.Queries.Offices.GetOfficeStatistics;

using AutoMapper;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Offices;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Interfaces.Repositories;

public class GetOfficeStatisticsQueryHandler : IRequestHandler<GetOfficeStatisticsQuery, Result<OfficeStatisticsDto>>
{
    private readonly IOfficeRepository _officeRepository;
    private readonly ISiteRepository _siteRepository;
    private readonly IUserRepository _userRepository;
    private readonly IVisitRepository _visitRepository;
    private readonly IMaterialRepository _materialRepository;
    private readonly IMapper _mapper;

    public GetOfficeStatisticsQueryHandler(
        IOfficeRepository officeRepository,
        ISiteRepository siteRepository,
        IUserRepository userRepository,
        IVisitRepository visitRepository,
        IMaterialRepository materialRepository,
        IMapper mapper)
    {
        _officeRepository = officeRepository;
        _siteRepository = siteRepository;
        _userRepository = userRepository;
        _visitRepository = visitRepository;
        _materialRepository = materialRepository;
        _mapper = mapper;
    }

    public async Task<Result<OfficeStatisticsDto>> Handle(GetOfficeStatisticsQuery request, CancellationToken cancellationToken)
    {
        var office = await _officeRepository.GetByIdAsNoTrackingAsync(request.OfficeId, cancellationToken);
        if (office == null)
            return Result.Failure<OfficeStatisticsDto>("Office not found");

        // Get sites
        var sites = await _siteRepository.GetByOfficeIdAsNoTrackingAsync(request.OfficeId, cancellationToken);
        
        // Get users
        var users = await _userRepository.GetByOfficeIdAsNoTrackingAsync(request.OfficeId, cancellationToken);
        var engineers = users.Where(u => u.Role == UserRole.PMEngineer && u.IsActive).ToList();
        var technicians = users.Where(u => u.Role == UserRole.Technician && u.IsActive).ToList();
        
        // Get visits (approximate - would need proper query)
        var allVisits = await _visitRepository.GetByStatusAsNoTrackingAsync(Domain.Enums.VisitStatus.Scheduled, cancellationToken);
        var scheduledVisits = allVisits.Where(v => engineers.Any(e => e.Id == v.EngineerId)).Count();
        
        // Get materials
        var materials = await _materialRepository.GetByOfficeIdAsNoTrackingAsync(request.OfficeId, cancellationToken);
        var lowStockMaterials = materials.Where(m => m.IsStockLow()).Count();

        var statistics = new OfficeStatisticsDto
        {
            OfficeId = office.Id,
            OfficeCode = office.Code,
            OfficeName = office.Name,
            TotalSites = sites.Count(s => !s.IsDeleted),
            ActiveSites = sites.Count(s => !s.IsDeleted && s.Status == Domain.Enums.SiteStatus.OnAir),
            TotalEngineers = engineers.Count,
            ActiveEngineers = engineers.Count,
            TotalTechnicians = technicians.Count,
            ActiveTechnicians = technicians.Count,
            ScheduledVisits = scheduledVisits,
            TotalMaterials = materials.Count(m => m.IsActive),
            LowStockMaterials = lowStockMaterials,
            Region = office.Region
        };

        return Result.Success(statistics);
    }
}

