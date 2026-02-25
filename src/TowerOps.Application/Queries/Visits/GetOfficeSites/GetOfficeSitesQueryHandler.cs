namespace TowerOps.Application.Queries.Sites.GetOfficeSites;

using AutoMapper;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Sites;
using TowerOps.Domain.Interfaces.Repositories;
using TowerOps.Domain.Specifications.SiteSpecifications;

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
        var safePageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var safePageSize = Math.Clamp(request.PageSize, 1, 200);
        var skip = (safePageNumber - 1) * safePageSize;

        var countSpec = new OfficeSitesFilteredSpecification(
            request.OfficeId,
            request.Complexity,
            request.Status);
        var pagedSpec = new OfficeSitesFilteredSpecification(
            request.OfficeId,
            request.Complexity,
            request.Status,
            skip,
            safePageSize);

        var totalCount = await _siteRepository.CountAsync(countSpec, cancellationToken);
        var sites = await _siteRepository.FindAsNoTrackingAsync(pagedSpec, cancellationToken);

        var dtos = _mapper.Map<List<SiteDto>>(sites);
        var paginatedList = new PaginatedList<SiteDto>(dtos, totalCount, safePageNumber, safePageSize);

        return Result.Success(paginatedList);
    }
}
