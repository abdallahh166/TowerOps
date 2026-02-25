namespace TowerOps.Application.Queries.Visits.GetVisitsByDateRange;

using System;
using System.Collections.Generic;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Visits;

public record GetVisitsByDateRangeQuery : IQuery<List<VisitDto>>
{
    public DateTime FromDate { get; init; }
    public DateTime ToDate { get; init; }
    public Guid? EngineerId { get; init; }
    public Guid? SiteId { get; init; }
}

