namespace TowerOps.Application.Queries.Sites.GetSitesNeedingMaintenance;

using AutoMapper;
using MediatR;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Sites;
using TowerOps.Domain.Interfaces.Repositories;

public class GetSitesNeedingMaintenanceQueryHandler : IRequestHandler<GetSitesNeedingMaintenanceQuery, Result<List<SiteDto>>>
{
    private readonly ISiteRepository _siteRepository;
    private readonly IMapper _mapper;

    public GetSitesNeedingMaintenanceQueryHandler(ISiteRepository siteRepository, IMapper mapper)
    {
        _siteRepository = siteRepository;
        _mapper = mapper;
    }

    public async Task<Result<List<SiteDto>>> Handle(GetSitesNeedingMaintenanceQuery request, CancellationToken cancellationToken)
    {
        var sites = await _siteRepository.GetSitesNeedingMaintenanceAsync(request.DaysThreshold, cancellationToken);

        if (request.OfficeId.HasValue)
        {
            sites = sites.Where(s => s.OfficeId == request.OfficeId.Value).ToList();
        }

        var dtos = _mapper.Map<List<SiteDto>>(sites);
        return Result.Success(dtos);
    }
}