namespace TelecomPM.Application.Commands.Materials.UpdateMaterial;

using System;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Materials;
using TelecomPM.Domain.Enums;

public record UpdateMaterialCommand : ICommand<MaterialDto>
{
    public Guid MaterialId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public MaterialCategory Category { get; init; }
}

