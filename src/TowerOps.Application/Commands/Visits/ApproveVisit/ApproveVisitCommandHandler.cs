namespace TowerOps.Application.Commands.Visits.ApproveVisit;

using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using TowerOps.Application.Common;
using TowerOps.Application.Common.Interfaces;
using TowerOps.Domain.Enums;
using TowerOps.Domain.Exceptions;
using TowerOps.Domain.Interfaces.Repositories;
using TowerOps.Application.Services;

public class ApproveVisitCommandHandler : IRequestHandler<ApproveVisitCommand, Result>
{
    private readonly IVisitRepository _visitRepository;
    private readonly IUserRepository _userRepository;
    private readonly ISiteRepository _siteRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IVisitApprovalPolicyService _approvalPolicyService;
    private readonly ICurrentUserService _currentUserService;

    public ApproveVisitCommandHandler(
        IVisitRepository visitRepository,
        IUserRepository userRepository,
        ISiteRepository siteRepository,
        IUnitOfWork unitOfWork,
        IVisitApprovalPolicyService approvalPolicyService,
        ICurrentUserService currentUserService)
    {
        _visitRepository = visitRepository;
        _userRepository = userRepository;
        _siteRepository = siteRepository;
        _unitOfWork = unitOfWork;
        _approvalPolicyService = approvalPolicyService;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(ApproveVisitCommand request, CancellationToken cancellationToken)
    {
        var visit = await _visitRepository.GetByIdAsync(request.VisitId, cancellationToken);
        if (visit == null)
            return Result.Failure("Visit not found");

        if (!_currentUserService.IsAuthenticated || _currentUserService.UserId == Guid.Empty)
            return Result.Failure("Authenticated reviewer is required");

        var reviewer = await _userRepository.GetByIdAsync(_currentUserService.UserId, cancellationToken);
        if (reviewer == null)
            return Result.Failure("Reviewer not found");

        var policy = _approvalPolicyService.CanReviewVisit(reviewer.Role, visit.Type, ApprovalAction.Approved);
        if (!policy.IsAllowed)
            return Result.Failure(policy.DenialReason ?? "Reviewer is not allowed to approve this visit");

        try
        {
            if (visit.Status == VisitStatus.Submitted)
            {
                visit.StartReview();
            }

            visit.Approve(reviewer.Id, reviewer.Name, request.Notes);

            // Update site last visit date
            var site = await _siteRepository.GetByIdAsync(visit.SiteId, cancellationToken);
            if (site != null)
            {
                site.RecordVisit(visit.ActualStartTime ?? DateTime.UtcNow);
                await _siteRepository.UpdateAsync(site, cancellationToken);
            }

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
