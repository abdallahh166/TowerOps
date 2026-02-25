namespace TowerOps.Application.Commands.Offices.UpdateOffice;

using System;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Offices;

public record UpdateOfficeCommand : ICommand<OfficeDto>
{
    public Guid OfficeId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Region { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string Street { get; init; } = string.Empty;
    public string? BuildingNumber { get; init; }
    public string? PostalCode { get; init; }
}

