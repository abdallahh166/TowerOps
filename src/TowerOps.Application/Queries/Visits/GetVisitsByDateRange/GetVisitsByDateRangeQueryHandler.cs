namespace TowerOps.Application.Queries.Visits.GetVisitsByDateRange;

using AutoMapper;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Visits;
using TowerOps.Domain.Interfaces.Repositories;
using TowerOps.Domain.Specifications.VisitSpecifications;

public class GetVisitsByDateRangeQueryHandler : IRequestHandler<GetVisitsByDateRangeQuery, Result<List<VisitDto>>>
{
    private readonly IVisitRepository _visitRepository;
    private readonly IMapper _mapper;

    public GetVisitsByDateRangeQueryHandler(
        IVisitRepository visitRepository,
        IMapper mapper)
    {
        _visitRepository = visitRepository;
        _mapper = mapper;
    }

    public async Task<Result<List<VisitDto>>> Handle(GetVisitsByDateRangeQuery request, CancellationToken cancellationToken)
    {
        var spec = new VisitsByDateRangeSpecification(
            request.FromDate,
            request.ToDate,
            request.EngineerId,
            request.SiteId);
        var visits = await _visitRepository.FindAsNoTrackingAsync(spec, cancellationToken);

        var dtos = _mapper.Map<List<VisitDto>>(visits);
        return Result.Success(dtos);
    }
}

