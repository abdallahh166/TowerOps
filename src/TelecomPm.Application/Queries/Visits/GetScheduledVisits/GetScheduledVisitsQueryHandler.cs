namespace TelecomPM.Application.Queries.Visits.GetScheduledVisits;

using AutoMapper;
using MediatR;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Visits;
using TelecomPM.Domain.Interfaces.Repositories;
using TelecomPM.Domain.Specifications.VisitSpecifications;

public class GetScheduledVisitsQueryHandler : IRequestHandler<GetScheduledVisitsQuery, Result<List<VisitDto>>>
{
    private readonly IVisitRepository _visitRepository;
    private readonly IMapper _mapper;

    public GetScheduledVisitsQueryHandler(IVisitRepository visitRepository, IMapper mapper)
    {
        _visitRepository = visitRepository;
        _mapper = mapper;
    }

    public async Task<Result<List<VisitDto>>> Handle(GetScheduledVisitsQuery request, CancellationToken cancellationToken)
    {
        var spec = new ScheduledVisitsForDateSpecification(request.Date);
        var visits = await _visitRepository.FindAsync(spec, cancellationToken);

        if (request.EngineerId.HasValue)
        {
            visits = visits.Where(v => v.EngineerId == request.EngineerId.Value).ToList();
        }

        var dtos = _mapper.Map<List<VisitDto>>(visits);
        return Result.Success(dtos);
    }
}