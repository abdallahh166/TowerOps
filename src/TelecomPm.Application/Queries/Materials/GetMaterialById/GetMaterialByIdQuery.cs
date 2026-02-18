namespace TelecomPM.Application.Queries.Materials.GetMaterialById;

using System;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Materials;

public record GetMaterialByIdQuery : IQuery<MaterialDetailDto>
{
    public Guid MaterialId { get; init; }
}

