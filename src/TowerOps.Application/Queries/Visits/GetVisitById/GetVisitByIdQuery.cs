namespace TowerOps.Application.Queries.Visits.GetVisitById;

using System;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Visits;

public record GetVisitByIdQuery : IQuery<VisitDetailDto>
{
    public Guid VisitId { get; init; }
}