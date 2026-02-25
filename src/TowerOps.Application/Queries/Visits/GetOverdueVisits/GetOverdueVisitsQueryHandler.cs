namespace TowerOps.Application.Queries.Visits.GetOverdueVisits;

using AutoMapper;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Visits;
using TowerOps.Domain.Interfaces.Repositories;
using TowerOps.Domain.Specifications.VisitSpecifications;

public class GetOverdueVisitsQueryHandler : IRequestHandler<GetOverdueVisitsQuery, Result<List<VisitDto>>>
{
    private readonly IVisitRepository _visitRepository;
    private readonly IMapper _mapper;

    public GetOverdueVisitsQueryHandler(
        IVisitRepository visitRepository,
        IMapper mapper)
    {
        _visitRepository = visitRepository;
        _mapper = mapper;
    }

    public async Task<Result<List<VisitDto>>> Handle(GetOverdueVisitsQuery request, CancellationToken cancellationToken)
    {
        var spec = new OverdueVisitsSpecification(request.EngineerId);
        var visits = await _visitRepository.FindAsNoTrackingAsync(spec, cancellationToken);

        var dtos = _mapper.Map<List<VisitDto>>(visits);
        return Result.Success(dtos);
    }
}

