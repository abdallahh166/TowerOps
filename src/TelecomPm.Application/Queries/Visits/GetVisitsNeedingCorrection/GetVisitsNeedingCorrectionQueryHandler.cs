namespace TelecomPM.Application.Queries.Visits.GetVisitsNeedingCorrection;

using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Visits;
using TelecomPM.Domain.Interfaces.Repositories;
using TelecomPM.Domain.Specifications.VisitSpecifications;

public class GetVisitsNeedingCorrectionQueryHandler : IRequestHandler<GetVisitsNeedingCorrectionQuery, Result<List<VisitDto>>>
{
    private readonly IVisitRepository _visitRepository;
    private readonly IMapper _mapper;

    public GetVisitsNeedingCorrectionQueryHandler(
        IVisitRepository visitRepository,
        IMapper mapper)
    {
        _visitRepository = visitRepository;
        _mapper = mapper;
    }

    public async Task<Result<List<VisitDto>>> Handle(GetVisitsNeedingCorrectionQuery request, CancellationToken cancellationToken)
    {
        // VisitsNeedingCorrectionSpecification requires an engineerId, use Empty if not provided
        var spec = request.EngineerId.HasValue 
            ? new VisitsNeedingCorrectionSpecification(request.EngineerId.Value)
            : new VisitsNeedingCorrectionSpecification(Guid.Empty);
            
        var visits = await _visitRepository.FindAsync(spec, cancellationToken);

        // Filter by engineer if provided (spec handles it but let's be explicit)
        var filtered = visits.AsEnumerable();
        if (request.EngineerId.HasValue)
        {
            filtered = filtered.Where(v => v.EngineerId == request.EngineerId.Value);
        }

        var dtos = _mapper.Map<List<VisitDto>>(filtered.ToList());
        return Result.Success(dtos);
    }
}

