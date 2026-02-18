namespace TelecomPM.Application.Services;

using TelecomPM.Domain.Enums;

public interface IVisitApprovalPolicyService
{
    (bool IsAllowed, string? DenialReason) CanReviewVisit(UserRole reviewerRole, VisitType visitType, ApprovalAction action);
}
