namespace TowerOps.Application.Commands.Materials.DeleteMaterial;

using System;
using TowerOps.Application.Common;

public record DeleteMaterialCommand : ICommand
{
    public Guid MaterialId { get; init; }
    public string DeletedBy { get; init; } = string.Empty;
}

