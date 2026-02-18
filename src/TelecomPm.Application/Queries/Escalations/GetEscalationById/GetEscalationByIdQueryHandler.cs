namespace TelecomPM.Application.Queries.Escalations.GetEscalationById;

using AutoMapper;
using MediatR;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Escalations;
using TelecomPM.Domain.Interfaces.Repositories;

public class GetEscalationByIdQueryHandler : IRequestHandler<GetEscalationByIdQuery, Result<EscalationDto>>
{
    private readonly IEscalationRepository _escalationRepository;
    private readonly IMapper _mapper;

    public GetEscalationByIdQueryHandler(IEscalationRepository escalationRepository, IMapper mapper)
    {
        _escalationRepository = escalationRepository;
        _mapper = mapper;
    }

    public async Task<Result<EscalationDto>> Handle(GetEscalationByIdQuery request, CancellationToken cancellationToken)
    {
        var escalation = await _escalationRepository.GetByIdAsync(request.EscalationId, cancellationToken);
        if (escalation == null)
            return Result.Failure<EscalationDto>("Escalation not found");

        return Result.Success(_mapper.Map<EscalationDto>(escalation));
    }
}
