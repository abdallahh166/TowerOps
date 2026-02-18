namespace TelecomPM.Application.DTOs.Offices;

using System;

public record OfficeStatisticsDto
{
    public Guid OfficeId { get; init; }
    public string OfficeCode { get; init; } = string.Empty;
    public string OfficeName { get; init; } = string.Empty;
    public string Region { get; init; } = string.Empty;
    public int TotalSites { get; init; }
    public int ActiveSites { get; init; }
    public int TotalEngineers { get; init; }
    public int ActiveEngineers { get; init; }
    public int TotalTechnicians { get; init; }
    public int ActiveTechnicians { get; init; }
    public int ScheduledVisits { get; init; }
    public int TotalMaterials { get; init; }
    public int LowStockMaterials { get; init; }
}

