namespace TowerOps.Application.Services;

using TowerOps.Domain.Enums;

public interface IVisitApprovalPolicyService
{
    (bool IsAllowed, string? DenialReason) CanReviewVisit(UserRole reviewerRole, VisitType visitType, ApprovalAction action);
}
