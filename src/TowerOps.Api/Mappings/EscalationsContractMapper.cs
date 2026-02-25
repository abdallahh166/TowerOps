namespace TowerOps.Api.Mappings;

using TowerOps.Api.Contracts.Escalations;
using TowerOps.Application.Commands.Escalations.ApproveEscalation;
using TowerOps.Application.Commands.Escalations.CloseEscalation;
using TowerOps.Application.Commands.Escalations.CreateEscalation;
using TowerOps.Application.Commands.Escalations.RejectEscalation;
using TowerOps.Application.Commands.Escalations.ReviewEscalation;
using TowerOps.Application.Queries.Escalations.GetEscalationById;

public static class EscalationsContractMapper
{
    public static CreateEscalationCommand ToCommand(this CreateEscalationRequest request)
        => new()
        {
            WorkOrderId = request.WorkOrderId,
            IncidentId = request.IncidentId,
            SiteCode = request.SiteCode,
            SlaClass = request.SlaClass,
            FinancialImpactEgp = request.FinancialImpactEgp,
            SlaImpactPercentage = request.SlaImpactPercentage,
            EvidencePackage = request.EvidencePackage,
            PreviousActions = request.PreviousActions,
            RecommendedDecision = request.RecommendedDecision,
            Level = request.Level,
            SubmittedBy = request.SubmittedBy
        };

    public static GetEscalationByIdQuery ToEscalationByIdQuery(this Guid escalationId)
        => new() { EscalationId = escalationId };

    public static ReviewEscalationCommand ToReviewEscalationCommand(this Guid escalationId)
        => new() { EscalationId = escalationId };

    public static ApproveEscalationCommand ToApproveEscalationCommand(this Guid escalationId)
        => new() { EscalationId = escalationId };

    public static RejectEscalationCommand ToRejectEscalationCommand(this Guid escalationId)
        => new() { EscalationId = escalationId };

    public static CloseEscalationCommand ToCloseEscalationCommand(this Guid escalationId)
        => new() { EscalationId = escalationId };
}
