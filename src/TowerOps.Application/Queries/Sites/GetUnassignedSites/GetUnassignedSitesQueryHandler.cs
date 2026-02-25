namespace TowerOps.Application.Queries.Sites.GetUnassignedSites;

using AutoMapper;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Sites;
using TowerOps.Domain.Interfaces.Repositories;
using TowerOps.Domain.Specifications.SiteSpecifications;

public class GetUnassignedSitesQueryHandler : IRequestHandler<GetUnassignedSitesQuery, Result<List<SiteDto>>>
{
    private readonly ISiteRepository _siteRepository;
    private readonly IMapper _mapper;

    public GetUnassignedSitesQueryHandler(
        ISiteRepository siteRepository,
        IMapper mapper)
    {
        _siteRepository = siteRepository;
        _mapper = mapper;
    }

    public async Task<Result<List<SiteDto>>> Handle(GetUnassignedSitesQuery request, CancellationToken cancellationToken)
    {
        var spec = request.OfficeId.HasValue
            ? new UnassignedSitesSpecification(request.OfficeId.Value)
            : new UnassignedSitesSpecification();
            
        var sites = await _siteRepository.FindAsync(spec, cancellationToken);

        var dtos = _mapper.Map<List<SiteDto>>(sites.ToList());
        return Result.Success(dtos);
    }
}

