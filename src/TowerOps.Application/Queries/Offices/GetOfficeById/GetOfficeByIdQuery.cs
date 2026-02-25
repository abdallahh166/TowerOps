namespace TowerOps.Application.Queries.Offices.GetOfficeById;

using System;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Offices;

public record GetOfficeByIdQuery : IQuery<OfficeDetailDto>
{
    public Guid OfficeId { get; init; }
}

