namespace TelecomPM.Application.Queries.Sites.GetOfficeSites;

using AutoMapper;
using MediatR;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Sites;
using TelecomPM.Domain.Interfaces.Repositories;
using TelecomPM.Domain.Specifications.SiteSpecifications;

public class GetOfficeSitesQueryHandler : IRequestHandler<GetOfficeSitesQuery, Result<PaginatedList<SiteDto>>>
{
    private readonly ISiteRepository _siteRepository;
    private readonly IMapper _mapper;

    public GetOfficeSitesQueryHandler(ISiteRepository siteRepository, IMapper mapper)
    {
        _siteRepository = siteRepository;
        _mapper = mapper;
    }

    public async Task<Result<PaginatedList<SiteDto>>> Handle(GetOfficeSitesQuery request, CancellationToken cancellationToken)
    {
        var spec = new SitesByOfficeSpecification(request.OfficeId);
        var sites = await _siteRepository.FindAsync(spec, cancellationToken);

        // Apply filters
        var filtered = sites.AsQueryable();

        if (request.Complexity.HasValue)
        {
            filtered = filtered.Where(s => s.Complexity == request.Complexity.Value);
        }

        if (request.Status.HasValue)
        {
            filtered = filtered.Where(s => s.Status == request.Status.Value);
        }

        var dtos = _mapper.Map<List<SiteDto>>(filtered.ToList());
        var paginatedList = PaginatedList<SiteDto>.Create(dtos, request.PageNumber, request.PageSize);

        return Result.Success(paginatedList);
    }
}