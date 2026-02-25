namespace TowerOps.Application.Queries.Visits.GetPendingReviews;

using System;
using System.Collections.Generic;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Visits;

public record GetPendingReviewsQuery : IQuery<List<VisitDto>>
{
    public Guid? OfficeId { get; init; }
}