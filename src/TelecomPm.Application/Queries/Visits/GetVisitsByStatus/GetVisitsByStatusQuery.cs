namespace TelecomPM.Application.Queries.Visits.GetVisitsByStatus;

using System;
using System.Collections.Generic;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Visits;
using TelecomPM.Domain.Enums;

public record GetVisitsByStatusQuery : IQuery<List<VisitDto>>
{
    public VisitStatus Status { get; init; }
    public Guid? EngineerId { get; init; }
}

