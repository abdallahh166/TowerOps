namespace TowerOps.Application.Queries.Materials.GetMaterialsByOffice;

using System;
using System.Collections.Generic;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Materials;

public record GetMaterialsByOfficeQuery : IQuery<List<MaterialDto>>
{
    public Guid OfficeId { get; init; }
    public bool? OnlyInStock { get; init; }
}

