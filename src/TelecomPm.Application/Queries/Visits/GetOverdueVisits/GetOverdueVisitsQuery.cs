namespace TelecomPM.Application.Queries.Visits.GetOverdueVisits;

using System;
using System.Collections.Generic;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Visits;

public record GetOverdueVisitsQuery : IQuery<List<VisitDto>>
{
    public Guid? EngineerId { get; init; }
    public Guid? OfficeId { get; init; }
}

