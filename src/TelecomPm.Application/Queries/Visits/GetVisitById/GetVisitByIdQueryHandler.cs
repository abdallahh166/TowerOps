namespace TelecomPM.Application.Queries.Visits.GetVisitById;

using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Visits;
using TelecomPM.Domain.Interfaces.Repositories;

public class GetVisitByIdQueryHandler : IRequestHandler<GetVisitByIdQuery, Result<VisitDetailDto>>
{
    private readonly IVisitRepository _visitRepository;
    private readonly IMapper _mapper;

    public GetVisitByIdQueryHandler(IVisitRepository visitRepository, IMapper mapper)
    {
        _visitRepository = visitRepository;
        _mapper = mapper;
    }

    public async Task<Result<VisitDetailDto>> Handle(GetVisitByIdQuery request, CancellationToken cancellationToken)
    {
        var visit = await _visitRepository.GetByIdAsync(request.VisitId, cancellationToken);
        if (visit == null)
            return Result.Failure<VisitDetailDto>("Visit not found");

        var dto = _mapper.Map<VisitDetailDto>(visit);
        return Result.Success(dto);
    }
}