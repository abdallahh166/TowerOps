namespace TelecomPM.Application.DTOs.Kpi;

using TelecomPM.Domain.Enums;

public record OperationsKpiDashboardDto
{
    public DateTime GeneratedAtUtc { get; init; }
    public DateTime? FromDateUtc { get; init; }
    public DateTime? ToDateUtc { get; init; }
    public string? OfficeCode { get; init; }
    public SlaClass? SlaClass { get; init; }

    public int TotalWorkOrders { get; init; }
    public int OpenWorkOrders { get; init; }
    public int BreachedWorkOrders { get; init; }
    public int AtRiskWorkOrders { get; init; }
    public decimal SlaCompliancePercentage { get; init; }

    public int TotalReviewedVisits { get; init; }
    public decimal FtfRatePercent { get; init; }
    public decimal MttrHours { get; init; }
    public decimal ReopenRatePercent { get; init; }
    public decimal EvidenceCompletenessPercent { get; init; }

    // Backward-compatible aliases
    public decimal FirstTimeFixPercentage { get; init; }
    public decimal ReopenRatePercentage { get; init; }
    public decimal EvidenceCompletenessPercentage { get; init; }
    public decimal MeanTimeToRepairHours { get; init; }
}
