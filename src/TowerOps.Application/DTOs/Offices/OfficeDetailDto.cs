namespace TowerOps.Application.DTOs.Offices;

using System;
using System.Collections.Generic;

public record OfficeDetailDto : OfficeDto
{
    public string Street { get; init; } = string.Empty;
    public string? BuildingNumber { get; init; }
    public string? PostalCode { get; init; }
    public double? Latitude { get; init; }
    public double? Longitude { get; init; }
    public string ContactPerson { get; init; } = string.Empty;
    public string ContactPhone { get; init; } = string.Empty;
    public string ContactEmail { get; init; } = string.Empty;
    public int ActiveTechnicians { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

