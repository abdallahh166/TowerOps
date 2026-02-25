namespace TowerOps.Application.Commands.Materials.UpdateMaterial;

using System;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Materials;
using TowerOps.Domain.Enums;

public record UpdateMaterialCommand : ICommand<MaterialDto>
{
    public Guid MaterialId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public MaterialCategory Category { get; init; }
}

