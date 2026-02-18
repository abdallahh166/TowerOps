namespace TelecomPM.Application.Queries.Sites.GetSitesByRegion;

using AutoMapper;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Sites;
using TelecomPM.Domain.Interfaces.Repositories;
using TelecomPM.Domain.Specifications.SiteSpecifications;

public class GetSitesByRegionQueryHandler : IRequestHandler<GetSitesByRegionQuery, Result<List<SiteDto>>>
{
    private readonly ISiteRepository _siteRepository;
    private readonly IMapper _mapper;

    public GetSitesByRegionQueryHandler(
        ISiteRepository siteRepository,
        IMapper mapper)
    {
        _siteRepository = siteRepository;
        _mapper = mapper;
    }

    public async Task<Result<List<SiteDto>>> Handle(GetSitesByRegionQuery request, CancellationToken cancellationToken)
    {
        var spec = new SitesByRegionSpecification(request.Region, request.SubRegion);
        var sites = await _siteRepository.FindAsync(spec, cancellationToken);

        var dtos = _mapper.Map<List<SiteDto>>(sites.ToList());
        return Result.Success(dtos);
    }
}

