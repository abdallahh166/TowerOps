namespace TowerOps.Application.Queries.Sites.GetSiteById;

using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Sites;
using TowerOps.Domain.Interfaces.Repositories;
using TowerOps.Domain.Specifications.SiteSpecifications;

public class GetSiteByIdQueryHandler : IRequestHandler<GetSiteByIdQuery, Result<SiteDetailDto>>
{
    private readonly ISiteRepository _siteRepository;
    private readonly IMapper _mapper;

    public GetSiteByIdQueryHandler(ISiteRepository siteRepository, IMapper mapper)
    {
        _siteRepository = siteRepository;
        _mapper = mapper;
    }

    public async Task<Result<SiteDetailDto>> Handle(GetSiteByIdQuery request, CancellationToken cancellationToken)
    {
        var spec = new SiteWithFullDetailsSpecification(request.SiteId);
        var site = await _siteRepository.FindOneAsync(spec, cancellationToken);

        if (site == null)
            return Result.Failure<SiteDetailDto>("Site not found");

        var dto = _mapper.Map<SiteDetailDto>(site);
        return Result.Success(dto);
    }
}