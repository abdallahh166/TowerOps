namespace TowerOps.Application.Queries.Visits.GetVisitsByStatus;

using System;
using System.Collections.Generic;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Visits;
using TowerOps.Domain.Enums;

public record GetVisitsByStatusQuery : IQuery<List<VisitDto>>
{
    public VisitStatus Status { get; init; }
    public Guid? EngineerId { get; init; }
}

