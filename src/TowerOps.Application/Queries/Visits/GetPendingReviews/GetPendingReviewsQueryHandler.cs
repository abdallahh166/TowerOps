namespace TowerOps.Application.Queries.Visits.GetPendingReviews;

using AutoMapper;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Visits;
using TowerOps.Domain.Interfaces.Repositories;
using TowerOps.Domain.Specifications.VisitSpecifications;

public class GetPendingReviewsQueryHandler : IRequestHandler<GetPendingReviewsQuery, Result<List<VisitDto>>>
{
    private readonly IVisitRepository _visitRepository;
    private readonly IMapper _mapper;

    public GetPendingReviewsQueryHandler(IVisitRepository visitRepository, IMapper mapper)
    {
        _visitRepository = visitRepository;
        _mapper = mapper;
    }

    public async Task<Result<List<VisitDto>>> Handle(GetPendingReviewsQuery request, CancellationToken cancellationToken)
    {
        var spec = new PendingReviewVisitsSpecification();
        var visits = await _visitRepository.FindAsync(spec, cancellationToken);

        var dtos = _mapper.Map<List<VisitDto>>(visits);
        return Result.Success(dtos);
    }
}