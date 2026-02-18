namespace TelecomPM.Application.Queries.Visits.GetVisitsByDateRange;

using AutoMapper;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Visits;
using TelecomPM.Domain.Interfaces.Repositories;
using TelecomPM.Domain.Specifications.VisitSpecifications;

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
        var spec = new VisitsByDateRangeSpecification(request.FromDate, request.ToDate);
        var visits = await _visitRepository.FindAsync(spec, cancellationToken);

        var filtered = visits.AsQueryable();

        if (request.EngineerId.HasValue)
        {
            filtered = filtered.Where(v => v.EngineerId == request.EngineerId.Value);
        }

        if (request.SiteId.HasValue)
        {
            filtered = filtered.Where(v => v.SiteId == request.SiteId.Value);
        }

        var dtos = _mapper.Map<List<VisitDto>>(filtered.ToList());
        return Result.Success(dtos);
    }
}

