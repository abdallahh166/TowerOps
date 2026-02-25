using System;

namespace TowerOps.Application.DTOs.Offices;

public record OfficeDto
{
    public Guid Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Region { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public int TotalSites { get; init; }
    public int ActiveEngineers { get; init; }
    public bool IsActive { get; init; }
}