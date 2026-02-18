namespace TelecomPM.Application.Commands.Visits.RejectVisit;

using MediatR;
using System.Threading;
using System.Threading.Tasks;
using TelecomPM.Application.Common;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Exceptions;
using TelecomPM.Domain.Interfaces.Repositories;
using TelecomPM.Application.Services;

public class RejectVisitCommandHandler : IRequestHandler<RejectVisitCommand, Result>
{
    private readonly IVisitRepository _visitRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IVisitApprovalPolicyService _approvalPolicyService;

    public RejectVisitCommandHandler(
        IVisitRepository visitRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IVisitApprovalPolicyService approvalPolicyService)
    {
        _visitRepository = visitRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _approvalPolicyService = approvalPolicyService;
    }

    public async Task<Result> Handle(RejectVisitCommand request, CancellationToken cancellationToken)
    {
        var visit = await _visitRepository.GetByIdAsync(request.VisitId, cancellationToken);
        if (visit == null)
            return Result.Failure("Visit not found");

        var reviewer = await _userRepository.GetByIdAsync(request.ReviewerId, cancellationToken);
        if (reviewer == null)
            return Result.Failure("Reviewer not found");

        var policy = _approvalPolicyService.CanReviewVisit(reviewer.Role, visit.Type, ApprovalAction.Rejected);
        if (!policy.IsAllowed)
            return Result.Failure(policy.DenialReason ?? "Reviewer is not allowed to reject this visit");

        try
        {
            visit.Reject(reviewer.Id, reviewer.Name, request.RejectionReason);

            await _visitRepository.UpdateAsync(visit, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message);
        }
    }
}
