namespace TowerOps.Application.Queries.Materials.GetLowStockMaterials;

using System;
using System.Collections.Generic;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Materials;

public record GetLowStockMaterialsQuery : IQuery<List<MaterialDto>>
{
    public Guid OfficeId { get; init; }
}