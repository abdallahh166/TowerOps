namespace TelecomPM.Application.Queries.Materials.GetLowStockMaterials;

using System;
using System.Collections.Generic;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Materials;

public record GetLowStockMaterialsQuery : IQuery<List<MaterialDto>>
{
    public Guid OfficeId { get; init; }
}