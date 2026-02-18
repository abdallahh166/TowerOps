namespace TelecomPM.Application.Queries.Visits.GetEngineerVisits;

using System;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Visits;
using TelecomPM.Domain.Enums;

public record GetEngineerVisitsQuery : IQuery<PaginatedList<VisitDto>>
{
    public Guid EngineerId { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public VisitStatus? Status { get; init; }
    public DateTime? From { get; init; }
    public DateTime? To { get; init; }
}