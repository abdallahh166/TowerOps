namespace TelecomPM.Application.Queries.Visits.GetVisitById;

using System;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Visits;

public record GetVisitByIdQuery : IQuery<VisitDetailDto>
{
    public Guid VisitId { get; init; }
}