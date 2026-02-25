namespace TowerOps.Application.Queries.Materials.GetMaterialById;

using System;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Materials;

public record GetMaterialByIdQuery : IQuery<MaterialDetailDto>
{
    public Guid MaterialId { get; init; }
}

