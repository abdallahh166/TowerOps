namespace TowerOps.Application.Queries.Visits.GetOverdueVisits;

using System;
using System.Collections.Generic;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Visits;

public record GetOverdueVisitsQuery : IQuery<List<VisitDto>>
{
    public Guid? EngineerId { get; init; }
    public Guid? OfficeId { get; init; }
}

