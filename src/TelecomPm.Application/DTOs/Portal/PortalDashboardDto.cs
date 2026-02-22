namespace TelecomPM.Application.DTOs.Portal;

public sealed record PortalDashboardDto
{
    public int TotalSites { get; init; }
    public decimal OnAirPercent { get; init; }
    public decimal SlaComplianceRatePercent { get; init; }
    public int PendingCmCount { get; init; }
    public int OverdueBmCount { get; init; }
}
