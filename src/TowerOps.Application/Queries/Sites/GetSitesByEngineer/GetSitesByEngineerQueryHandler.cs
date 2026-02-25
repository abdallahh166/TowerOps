namespace TowerOps.Application.Queries.Sites.GetSitesByEngineer;

using AutoMapper;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Sites;
using TowerOps.Domain.Interfaces.Repositories;

public class GetSitesByEngineerQueryHandler : IRequestHandler<GetSitesByEngineerQuery, Result<List<SiteDto>>>
{
    private readonly ISiteRepository _siteRepository;
    private readonly IMapper _mapper;

    public GetSitesByEngineerQueryHandler(
        ISiteRepository siteRepository,
        IMapper mapper)
    {
        _siteRepository = siteRepository;
        _mapper = mapper;
    }

    public async Task<Result<List<SiteDto>>> Handle(GetSitesByEngineerQuery request, CancellationToken cancellationToken)
    {
        var sites = await _siteRepository.GetByEngineerIdAsNoTrackingAsync(request.EngineerId, cancellationToken);

        var dtos = _mapper.Map<List<SiteDto>>(sites.ToList());
        return Result.Success(dtos);
    }
}

