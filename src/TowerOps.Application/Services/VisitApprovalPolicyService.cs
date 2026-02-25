namespace TowerOps.Application.Services;

using TowerOps.Domain.Enums;

public sealed class VisitApprovalPolicyService : IVisitApprovalPolicyService
{
    public (bool IsAllowed, string? DenialReason) CanReviewVisit(UserRole reviewerRole, VisitType visitType, ApprovalAction action)
    {
        var isBmVisit = visitType.IsBm();

        if (isBmVisit)
        {
            var allowed = reviewerRole is UserRole.Manager or UserRole.Admin;
            return allowed
                ? (true, null)
                : (false, "BM approvals require manager or admin role");
        }

        var cmAllowed = reviewerRole is UserRole.Supervisor or UserRole.Manager or UserRole.Admin;
        return cmAllowed
            ? (true, null)
            : (false, "CM approvals require supervisor, manager, or admin role");
    }
}
