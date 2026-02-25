namespace TowerOps.Application.Commands.Offices.CreateOffice;

using System;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Offices;

public record CreateOfficeCommand : ICommand<OfficeDto>
{
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Region { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string Street { get; init; } = string.Empty;
    public string? BuildingNumber { get; init; }
    public string? PostalCode { get; init; }
    public double? Latitude { get; init; }
    public double? Longitude { get; init; }
    public string? ContactPerson { get; init; }
    public string? ContactPhone { get; init; }
    public string? ContactEmail { get; init; }
}

