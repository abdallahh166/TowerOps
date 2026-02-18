namespace TelecomPM.Application.Queries.Visits.GetScheduledVisits;

using System;
using System.Collections.Generic;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Visits;

public record GetScheduledVisitsQuery : IQuery<List<VisitDto>>
{
    public DateTime Date { get; init; }
    public Guid? EngineerId { get; init; }
}