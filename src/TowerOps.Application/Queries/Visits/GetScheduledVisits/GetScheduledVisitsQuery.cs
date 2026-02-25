namespace TowerOps.Application.Queries.Visits.GetScheduledVisits;

using System;
using System.Collections.Generic;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Visits;

public record GetScheduledVisitsQuery : IQuery<List<VisitDto>>
{
    public DateTime Date { get; init; }
    public Guid? EngineerId { get; init; }
}