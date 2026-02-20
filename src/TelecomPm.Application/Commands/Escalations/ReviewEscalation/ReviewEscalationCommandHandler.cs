namespace TelecomPM.Application.Commands.Escalations.ReviewEscalation;

using AutoMapper;
using MediatR;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Escalations;
using TelecomPM.Domain.Exceptions;
using TelecomPM.Domain.Interfaces.Repositories;

public class ReviewEscalationCommandHandler : IRequestHandler<ReviewEscalationCommand, Result<EscalationDto>>
{
    private readonly IEscalationRepository _escalationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ReviewEscalationCommandHandler(
        IEscalationRepository escalationRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _escalationRepository = escalationRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<EscalationDto>> Handle(ReviewEscalationCommand request, CancellationToken cancellationToken)
    {
        var escalation = await _escalationRepository.GetByIdAsync(request.EscalationId, cancellationToken);
        if (escalation is null)
            return Result.Failure<EscalationDto>("Escalation not found");

        try
        {
            escalation.MarkUnderReview();
            await _escalationRepository.UpdateAsync(escalation, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(_mapper.Map<EscalationDto>(escalation));
        }
        catch (DomainException ex)
        {
            return Result.Failure<EscalationDto>(ex.Message);
        }
    }
}
