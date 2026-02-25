using TowerOps.Domain.Enums;

namespace TowerOps.Application.DTOs.Portal;

public sealed record PortalSlaReportDto
{
    public IReadOnlyList<PortalSlaMonthlyMetricDto> Monthly { get; init; } = Array.Empty<PortalSlaMonthlyMetricDto>();
}

public sealed record PortalSlaMonthlyMetricDto
{
    public int Year { get; init; }
    public int Month { get; init; }
    public SlaClass SlaClass { get; init; }
    public decimal CompliancePercent { get; init; }
    public int BreachCount { get; init; }
    public decimal AverageResponseMinutes { get; init; }
}
