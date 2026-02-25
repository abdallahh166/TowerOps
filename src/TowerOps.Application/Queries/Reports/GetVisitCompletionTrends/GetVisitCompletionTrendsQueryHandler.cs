namespace TowerOps.Application.Queries.Reports.GetVisitCompletionTrends;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Reports;
using TowerOps.Domain.Enums;
using TowerOps.Domain.Interfaces.Repositories;
using TowerOps.Domain.Specifications.VisitSpecifications;

public class GetVisitCompletionTrendsQueryHandler 
    : IRequestHandler<GetVisitCompletionTrendsQuery, Result<List<VisitCompletionTrendDto>>>
{
    private readonly IVisitRepository _visitRepository;
    private readonly ISiteRepository _siteRepository;

    public GetVisitCompletionTrendsQueryHandler(
        IVisitRepository visitRepository,
        ISiteRepository siteRepository)
    {
        _visitRepository = visitRepository;
        _siteRepository = siteRepository;
    }

    public async Task<Result<List<VisitCompletionTrendDto>>> Handle(
        GetVisitCompletionTrendsQuery request,
        CancellationToken cancellationToken)
    {
        IReadOnlyCollection<Guid>? officeSiteIds = null;
        if (request.OfficeId.HasValue)
        {
            officeSiteIds = await _siteRepository.GetSiteIdsByOfficeAsNoTrackingAsync(request.OfficeId.Value, cancellationToken);
        }

        var visitSpec = new VisitsByDateRangeSpecification(
            request.FromDate,
            request.ToDate,
            request.EngineerId,
            siteIds: officeSiteIds);
        var allVisits = await _visitRepository.FindAsNoTrackingAsync(visitSpec, cancellationToken);

        var trends = new List<VisitCompletionTrendDto>();

        switch (request.Period)
        {
            case TrendPeriod.Daily:
                trends = allVisits
                    .GroupBy(v => v.ScheduledDate.Date)
                    .Select(g => CreateTrendDto(g.Key, g.ToList()))
                    .OrderBy(t => t.Period)
                    .ToList();
                break;

            case TrendPeriod.Weekly:
                trends = allVisits
                    .GroupBy(v => GetWeekStart(v.ScheduledDate))
                    .Select(g => CreateTrendDto(g.Key, g.ToList()))
                    .OrderBy(t => t.Period)
                    .ToList();
                break;

            case TrendPeriod.Monthly:
                trends = allVisits
                    .GroupBy(v => new DateTime(v.ScheduledDate.Year, v.ScheduledDate.Month, 1))
                    .Select(g => CreateTrendDto(g.Key, g.ToList()))
                    .OrderBy(t => t.Period)
                    .ToList();
                break;

            case TrendPeriod.Yearly:
                trends = allVisits
                    .GroupBy(v => new DateTime(v.ScheduledDate.Year, 1, 1))
                    .Select(g => CreateTrendDto(g.Key, g.ToList()))
                    .OrderBy(t => t.Period)
                    .ToList();
                break;
        }

        return Result.Success(trends);
    }

    private static VisitCompletionTrendDto CreateTrendDto(DateTime period, List<Domain.Entities.Visits.Visit> visits)
    {
        var totalVisits = visits.Count;
        var completedVisits = visits.Count(v => v.Status == VisitStatus.Completed);
        var approvedVisits = visits.Count(v => v.Status == VisitStatus.Approved);
        var rejectedVisits = visits.Count(v => v.Status == VisitStatus.Rejected);
        var overdueVisits = visits.Count(v => v.Status != VisitStatus.Completed && v.ScheduledDate < DateTime.UtcNow);

        var completionRate = totalVisits > 0 ? (decimal)completedVisits / totalVisits * 100 : 0;
        var approvalRate = completedVisits > 0 ? (decimal)approvedVisits / completedVisits * 100 : 0;

        return new VisitCompletionTrendDto
        {
            Period = period,
            TotalVisits = totalVisits,
            CompletedVisits = completedVisits,
            ApprovedVisits = approvedVisits,
            RejectedVisits = rejectedVisits,
            CompletionRate = Math.Round(completionRate, 2),
            ApprovalRate = Math.Round(approvalRate, 2),
            OverdueVisits = overdueVisits
        };
    }

    private static DateTime GetWeekStart(DateTime date)
    {
        var diff = (7 + (date.DayOfWeek - DayOfWeek.Sunday)) % 7;
        return date.AddDays(-1 * diff).Date;
    }
}

