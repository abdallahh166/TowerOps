
using AutoMapper;
using MediatR;
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
        var safePageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var safePageSize = Math.Clamp(request.PageSize, 1, 200);
        var skip = (safePageNumber - 1) * safePageSize;

        var countSpec = new EngineerVisitsFilteredSpecification(
            request.EngineerId,
            request.Status,
            request.From,
            request.To);
        var pagedSpec = new EngineerVisitsFilteredSpecification(
            request.EngineerId,
            request.Status,
            request.From,
            request.To,
            skip,
            safePageSize);

        var totalCount = await _visitRepository.CountAsync(countSpec, cancellationToken);
        var visits = await _visitRepository.FindAsNoTrackingAsync(pagedSpec, cancellationToken);

        var dtos = _mapper.Map<List<VisitDto>>(visits);
        var paginatedList = new PaginatedList<VisitDto>(dtos, totalCount, safePageNumber, safePageSize);

        return Result.Success(paginatedList);
    }
}
