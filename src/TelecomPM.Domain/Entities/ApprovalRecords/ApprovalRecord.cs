using TelecomPM.Domain.Common;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Exceptions;

namespace TelecomPM.Domain.Entities.ApprovalRecords;

public sealed class ApprovalRecord : AggregateRoot<Guid>
{
    public Guid WorkOrderId { get; private set; }
    public WorkflowType WorkflowType { get; private set; }
    public string StageName { get; private set; } = string.Empty;
    public string ReviewerRole { get; private set; } = string.Empty;
    public Guid ReviewerId { get; private set; }
    public ApprovalDecision Decision { get; private set; }
    public string? Reason { get; private set; }
    public DateTime DecisionAtUtc { get; private set; }

    private ApprovalRecord() : base() { }

    private ApprovalRecord(
        Guid workOrderId,
        WorkflowType workflowType,
        string stageName,
        string reviewerRole,
        Guid reviewerId,
        ApprovalDecision decision,
        string? reason) : base(Guid.NewGuid())
    {
        WorkOrderId = workOrderId;
        WorkflowType = workflowType;
        StageName = stageName;
        ReviewerRole = reviewerRole;
        ReviewerId = reviewerId;
        Decision = decision;
        Reason = reason;
        DecisionAtUtc = DateTime.UtcNow;
    }

    public static ApprovalRecord Create(
        Guid workOrderId,
        WorkflowType workflowType,
        string stageName,
        string reviewerRole,
        Guid reviewerId,
        ApprovalDecision decision,
        string? reason)
    {
        if (workOrderId == Guid.Empty)
            throw new DomainException("WorkOrderId is required", "ApprovalRecord.WorkOrderId.Required");

        if (string.IsNullOrWhiteSpace(stageName))
            throw new DomainException("Stage name is required", "ApprovalRecord.StageName.Required");

        if (string.IsNullOrWhiteSpace(reviewerRole))
            throw new DomainException("Reviewer role is required", "ApprovalRecord.ReviewerRole.Required");

        if (reviewerId == Guid.Empty)
            throw new DomainException("ReviewerId is required", "ApprovalRecord.ReviewerId.Required");

        return new ApprovalRecord(
            workOrderId,
            workflowType,
            stageName.Trim(),
            reviewerRole.Trim(),
            reviewerId,
            decision,
            reason);
    }
}
