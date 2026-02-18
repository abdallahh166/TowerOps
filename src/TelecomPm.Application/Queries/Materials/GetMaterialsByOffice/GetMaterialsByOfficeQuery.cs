namespace TelecomPM.Application.Queries.Materials.GetMaterialsByOffice;

using System;
using System.Collections.Generic;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Materials;

public record GetMaterialsByOfficeQuery : IQuery<List<MaterialDto>>
{
    public Guid OfficeId { get; init; }
    public bool? OnlyInStock { get; init; }
}

