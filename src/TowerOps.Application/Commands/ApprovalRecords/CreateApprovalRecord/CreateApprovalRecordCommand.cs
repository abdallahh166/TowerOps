namespace TowerOps.Application.Commands.ApprovalRecords.CreateApprovalRecord;

using TowerOps.Application.Common;
using TowerOps.Domain.Enums;

public record CreateApprovalRecordCommand : ICommand<Guid>
{
    public Guid WorkOrderId { get; init; }
    public WorkflowType WorkflowType { get; init; }
    public string StageName { get; init; } = string.Empty;
    public string ReviewerRole { get; init; } = string.Empty;
    public Guid ReviewerId { get; init; }
    public ApprovalDecision Decision { get; init; }
    public string? Reason { get; init; }
}
