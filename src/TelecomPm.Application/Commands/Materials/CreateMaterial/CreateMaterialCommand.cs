namespace TelecomPM.Application.Commands.Materials.CreateMaterial;

using System;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Materials;
using TelecomPM.Domain.Enums;

public record CreateMaterialCommand : ICommand<MaterialDto>
{
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public MaterialCategory Category { get; init; }
    public Guid OfficeId { get; init; }
    public decimal InitialStock { get; init; }
    public MaterialUnit Unit { get; init; }
    public decimal MinimumStock { get; init; }
    public decimal UnitCost { get; init; }
    public string Currency { get; init; } = "EGP";
    public string? Supplier { get; init; }
}

