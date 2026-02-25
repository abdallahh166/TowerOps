namespace TowerOps.Application.Queries.Materials.GetMaterialsByCategory;

using System;
using System.Collections.Generic;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Materials;
using TowerOps.Domain.Enums;

public record GetMaterialsByCategoryQuery : IQuery<List<MaterialDto>>
{
    public MaterialCategory Category { get; init; }
    public Guid? OfficeId { get; init; }
}

