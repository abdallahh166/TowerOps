namespace TelecomPM.Application.Commands.Escalations.CreateEscalation;

using AutoMapper;
using MediatR;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Escalations;
using TelecomPM.Application.Services;
using TelecomPM.Domain.Entities.Escalations;
using TelecomPM.Domain.Interfaces.Repositories;

public class CreateEscalationCommandHandler : IRequestHandler<CreateEscalationCommand, Result<EscalationDto>>
{
    private readonly IEscalationRepository _escalationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IEscalationRoutingService _escalationRoutingService;

    public CreateEscalationCommandHandler(
        IEscalationRepository escalationRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IEscalationRoutingService escalationRoutingService)
    {
        _escalationRepository = escalationRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _escalationRoutingService = escalationRoutingService;
    }

    public async Task<Result<EscalationDto>> Handle(CreateEscalationCommand request, CancellationToken cancellationToken)
    {
        var existing = await _escalationRepository.GetByIncidentIdAsync(request.IncidentId, cancellationToken);
        if (existing != null)
            return Result.Failure<EscalationDto>($"Escalation with incident {request.IncidentId} already exists");

        var expectedLevel = _escalationRoutingService.DetermineLevel(request.SlaClass, request.FinancialImpactEgp, request.SlaImpactPercentage);
        if (request.Level != expectedLevel)
            return Result.Failure<EscalationDto>($"Escalation level must be {expectedLevel} based on routing rules");

        var escalation = Escalation.Create(
            request.WorkOrderId,
            request.IncidentId,
            request.SiteCode,
            request.SlaClass,
            request.FinancialImpactEgp,
            request.SlaImpactPercentage,
            request.EvidencePackage,
            request.PreviousActions,
            request.RecommendedDecision,
            request.Level,
            request.SubmittedBy);

        await _escalationRepository.AddAsync(escalation, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(_mapper.Map<EscalationDto>(escalation));
    }
}
