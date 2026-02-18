using TelecomPM.Domain.Common;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Exceptions;

namespace TelecomPM.Domain.Entities.Visits;

public sealed class VisitApproval : Entity<Guid>
{
    public Guid VisitId { get; private set; }
    public Guid ReviewerId { get; private set; }
    public string ReviewerName { get; private set; } = string.Empty;
    public ApprovalAction Action { get; private set; }
    public string? Comments { get; private set; }
    public DateTime ReviewedAt { get; private set; }

    private VisitApproval() : base() { }

    private VisitApproval(
        Guid visitId,
        Guid reviewerId,
        string reviewerName,
        ApprovalAction action,
        string? comments) : base(Guid.NewGuid())
    {
        if (visitId == Guid.Empty)
            throw new DomainException("VisitId cannot be empty.");
        if (reviewerId == Guid.Empty)
            throw new DomainException("ReviewerId cannot be empty.");
        if (string.IsNullOrWhiteSpace(reviewerName))
            throw new DomainException("Reviewer name is required.");

        VisitId = visitId;
        ReviewerId = reviewerId;
        ReviewerName = reviewerName;
        Action = action;
        Comments = comments;
        ReviewedAt = DateTime.UtcNow;
    }

    public static VisitApproval Create(
        Guid visitId,
        Guid reviewerId,
        string reviewerName,
        ApprovalAction action,
        string? comments = null)
    {
        return new VisitApproval(visitId, reviewerId, reviewerName, action, comments);
    }
}
