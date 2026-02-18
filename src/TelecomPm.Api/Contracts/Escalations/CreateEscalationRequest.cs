namespace TelecomPm.Api.Contracts.Escalations;

using System.ComponentModel.DataAnnotations;
using TelecomPM.Domain.Enums;

public record CreateEscalationRequest
{
    [Required]
    public Guid WorkOrderId { get; init; }

    [Required]
    [MaxLength(100)]
    public string IncidentId { get; init; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string SiteCode { get; init; } = string.Empty;

    [Required]
    public SlaClass SlaClass { get; init; }

    [Range(0, double.MaxValue)]
    public decimal FinancialImpactEgp { get; init; }

    [Range(0, 100)]
    public decimal SlaImpactPercentage { get; init; }

    [Required]
    [MaxLength(4000)]
    public string EvidencePackage { get; init; } = string.Empty;

    [Required]
    [MaxLength(4000)]
    public string PreviousActions { get; init; } = string.Empty;

    [Required]
    [MaxLength(2000)]
    public string RecommendedDecision { get; init; } = string.Empty;

    [Required]
    public EscalationLevel Level { get; init; }

    [Required]
    [MaxLength(200)]
    public string SubmittedBy { get; init; } = string.Empty;
}
