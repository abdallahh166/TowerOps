namespace TelecomPM.Application.Queries.Visits.GetVisitsByDateRange;

using System;
using System.Collections.Generic;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Visits;

public record GetVisitsByDateRangeQuery : IQuery<List<VisitDto>>
{
    public DateTime FromDate { get; init; }
    public DateTime ToDate { get; init; }
    public Guid? EngineerId { get; init; }
    public Guid? SiteId { get; init; }
}

