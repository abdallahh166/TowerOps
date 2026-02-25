namespace TowerOps.Application.Queries.Visits.GetVisitsNeedingCorrection;

using System;
using System.Collections.Generic;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Visits;

public record GetVisitsNeedingCorrectionQuery : IQuery<List<VisitDto>>
{
    public Guid? EngineerId { get; init; }
}

