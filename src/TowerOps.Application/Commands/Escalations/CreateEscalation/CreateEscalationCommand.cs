namespace TowerOps.Application.Commands.Escalations.CreateEscalation;

using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Escalations;
using TowerOps.Domain.Enums;

public record CreateEscalationCommand : ICommand<EscalationDto>
{
    public Guid WorkOrderId { get; init; }
    public string IncidentId { get; init; } = string.Empty;
    public string SiteCode { get; init; } = string.Empty;
    public SlaClass SlaClass { get; init; }
    public decimal FinancialImpactEgp { get; init; }
    public decimal SlaImpactPercentage { get; init; }
    public string EvidencePackage { get; init; } = string.Empty;
    public string PreviousActions { get; init; } = string.Empty;
    public string RecommendedDecision { get; init; } = string.Empty;
    public EscalationLevel Level { get; init; }
    public string SubmittedBy { get; init; } = string.Empty;
}
