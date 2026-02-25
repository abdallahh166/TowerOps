namespace TowerOps.Application.DTOs.Escalations;

using TowerOps.Domain.Enums;
using System;
public record EscalationDto
{
    public Guid Id { get; init; }
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
    public EscalationStatus Status { get; init; }
    public string SubmittedBy { get; init; } = string.Empty;
    public DateTime SubmittedAtUtc { get; init; }
}
