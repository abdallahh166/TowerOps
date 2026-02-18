namespace TelecomPM.Application.Queries.Materials.GetMaterialsByCategory;

using System;
using System.Collections.Generic;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Materials;
using TelecomPM.Domain.Enums;

public record GetMaterialsByCategoryQuery : IQuery<List<MaterialDto>>
{
    public MaterialCategory Category { get; init; }
    public Guid? OfficeId { get; init; }
}

