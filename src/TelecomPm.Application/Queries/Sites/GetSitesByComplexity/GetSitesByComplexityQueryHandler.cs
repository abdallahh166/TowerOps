namespace TelecomPM.Application.Queries.Sites.GetSitesByComplexity;

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

public class GetSitesByComplexityQueryHandler : IRequestHandler<GetSitesByComplexityQuery, Result<List<SiteDto>>>
{
    private readonly ISiteRepository _siteRepository;
    private readonly IMapper _mapper;

    public GetSitesByComplexityQueryHandler(
        ISiteRepository siteRepository,
        IMapper mapper)
    {
        _siteRepository = siteRepository;
        _mapper = mapper;
    }

    public async Task<Result<List<SiteDto>>> Handle(GetSitesByComplexityQuery request, CancellationToken cancellationToken)
    {
        var spec = new SitesByComplexitySpecification(request.Complexity);
        var sites = await _siteRepository.FindAsync(spec, cancellationToken);

        var filtered = sites.AsQueryable();

        if (request.OfficeId.HasValue)
        {
            filtered = filtered.Where(s => s.OfficeId == request.OfficeId.Value);
        }

        var dtos = _mapper.Map<List<SiteDto>>(filtered.ToList());
        return Result.Success(dtos);
    }
}

