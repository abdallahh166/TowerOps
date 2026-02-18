namespace TelecomPM.Application.Queries.Visits.GetPendingReviews;

using System;
using System.Collections.Generic;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Visits;

public record GetPendingReviewsQuery : IQuery<List<VisitDto>>
{
    public Guid? OfficeId { get; init; }
}