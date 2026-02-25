namespace TowerOps.Application.Commands.Materials.ConsumeMaterial;

using System;
using TowerOps.Application.Common;
using TowerOps.Domain.Enums;

public record ConsumeMaterialCommand : ICommand
{
    public Guid MaterialId { get; init; }
    public Guid VisitId { get; init; }
    public string PerformedBy { get; init; } = string.Empty;
}

