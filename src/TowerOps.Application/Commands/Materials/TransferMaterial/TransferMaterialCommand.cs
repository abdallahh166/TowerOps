namespace TowerOps.Application.Commands.Materials.TransferMaterial;

using System;
using TowerOps.Application.Common;
using TowerOps.Domain.Enums;

public record TransferMaterialCommand : ICommand
{
    public Guid MaterialId { get; init; }
    public Guid FromOfficeId { get; init; }
    public Guid ToOfficeId { get; init; }
    public decimal Quantity { get; init; }
    public MaterialUnit Unit { get; init; }
    public string Reason { get; init; } = string.Empty;
    public string TransferredBy { get; init; } = string.Empty;
}

