namespace TelecomPM.Application.Queries.Visits.GetVisitsNeedingCorrection;

using System;
using System.Collections.Generic;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Visits;

public record GetVisitsNeedingCorrectionQuery : IQuery<List<VisitDto>>
{
    public Guid? EngineerId { get; init; }
}

