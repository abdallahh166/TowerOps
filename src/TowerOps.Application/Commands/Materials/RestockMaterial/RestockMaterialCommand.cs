namespace TowerOps.Application.Commands.Materials.RestockMaterial;

using System;
using TowerOps.Application.Common;
using TowerOps.Domain.Enums;

public record RestockMaterialCommand : ICommand
{
    public Guid MaterialId { get; init; }
    public decimal Quantity { get; init; }
    public MaterialUnit Unit { get; init; }
    public string RestockedBy { get; init; } = string.Empty;
    public string? Supplier { get; init; }
}

