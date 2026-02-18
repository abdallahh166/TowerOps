
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

namespace TelecomPM.Application.Queries.Visits.GetEngineerVisits;

public class GetEngineerVisitsQueryHandler : IRequestHandler<GetEngineerVisitsQuery, Result<PaginatedList<VisitDto>>>
{
    private readonly IVisitRepository _visitRepository;
    private readonly IMapper _mapper;

    public GetEngineerVisitsQueryHandler(IVisitRepository visitRepository, IMapper mapper)
    {
        _visitRepository = visitRepository;
        _mapper = mapper;
    }

    public async Task<Result<PaginatedList<VisitDto>>> Handle(GetEngineerVisitsQuery request, CancellationToken cancellationToken)
    {
        var spec = new VisitsByEngineerSpecification(request.EngineerId);
        var visits = await _visitRepository.FindAsync(spec, cancellationToken);

        // Apply filters
        var filtered = visits.AsQueryable();

        if (request.Status.HasValue)
        {
            filtered = filtered.Where(v => v.Status == request.Status.Value);
        }

        if (request.From.HasValue)
        {
            filtered = filtered.Where(v => v.ScheduledDate >= request.From.Value);
        }

        if (request.To.HasValue)
        {
            filtered = filtered.Where(v => v.ScheduledDate <= request.To.Value);
        }

        var dtos = _mapper.Map<List<VisitDto>>(filtered.ToList());
        var paginatedList = PaginatedList<VisitDto>.Create(dtos, request.PageNumber, request.PageSize);

        return Result.Success(paginatedList);
    }
}