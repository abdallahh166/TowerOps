namespace TelecomPM.Application.Queries.Visits.GetPendingReviews;

using AutoMapper;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Visits;
using TelecomPM.Domain.Interfaces.Repositories;
using TelecomPM.Domain.Specifications.VisitSpecifications;

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