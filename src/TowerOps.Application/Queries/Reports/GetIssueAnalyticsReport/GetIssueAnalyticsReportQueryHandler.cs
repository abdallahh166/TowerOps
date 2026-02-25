namespace TowerOps.Application.Queries.Reports.GetIssueAnalyticsReport;

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

public class GetIssueAnalyticsReportQueryHandler 
    : IRequestHandler<GetIssueAnalyticsReportQuery, Result<IssueAnalyticsReportDto>>
{
    private readonly IVisitRepository _visitRepository;
    private readonly ISiteRepository _siteRepository;

    public GetIssueAnalyticsReportQueryHandler(
        IVisitRepository visitRepository,
        ISiteRepository siteRepository)
    {
        _visitRepository = visitRepository;
        _siteRepository = siteRepository;
    }

    public async Task<Result<IssueAnalyticsReportDto>> Handle(
        GetIssueAnalyticsReportQuery request,
        CancellationToken cancellationToken)
    {
        var fromDate = request.FromDate ?? DateTime.UtcNow.AddMonths(-3);
        var toDate = request.ToDate ?? DateTime.UtcNow;

        IReadOnlyCollection<Guid>? officeSiteIds = null;
        if (request.OfficeId.HasValue)
        {
            officeSiteIds = await _siteRepository.GetSiteIdsByOfficeAsNoTrackingAsync(request.OfficeId.Value, cancellationToken);
        }

        var visitSpec = new VisitsByDateRangeSpecification(
            fromDate,
            toDate,
            siteId: request.SiteId,
            siteIds: officeSiteIds);
        var allVisits = await _visitRepository.FindAsNoTrackingAsync(visitSpec, cancellationToken);

        // Get all issues
        var allIssues = allVisits.SelectMany(v => v.IssuesFound).ToList();

        var totalIssues = allIssues.Count;
        var openIssues = allIssues.Count(i => i.Status != IssueStatus.Resolved && i.Status != IssueStatus.Closed);
        var resolvedIssues = allIssues.Count(i => i.Status == IssueStatus.Resolved || i.Status == IssueStatus.Closed);

        // By severity
        var criticalIssues = allIssues.Count(i => i.Severity == IssueSeverity.Critical);
        var highIssues = allIssues.Count(i => i.Severity == IssueSeverity.High);
        var mediumIssues = allIssues.Count(i => i.Severity == IssueSeverity.Medium);
        var lowIssues = allIssues.Count(i => i.Severity == IssueSeverity.Low);

        var resolutionRate = totalIssues > 0 ? (decimal)resolvedIssues / totalIssues * 100 : 0;

        // Average resolution time
        var resolvedWithTimes = allIssues
            .Where(i => (i.Status == IssueStatus.Resolved || i.Status == IssueStatus.Closed) && i.ResolvedAt.HasValue)
            .ToList();

        var avgResolutionTime = resolvedWithTimes.Any()
            ? resolvedWithTimes.Average(i => (i.ResolvedAt!.Value - i.CreatedAt).TotalDays)
            : 0;

        // Issues by category
        var issuesByCategory = allIssues
            .GroupBy(i => i.Category)
            .Select(g => new IssueByCategoryDto
            {
                Category = g.Key.ToString(),
                TotalCount = g.Count(),
                OpenCount = g.Count(i => i.Status != IssueStatus.Resolved && i.Status != IssueStatus.Closed),
                ResolvedCount = g.Count(i => i.Status == IssueStatus.Resolved || i.Status == IssueStatus.Closed)
            })
            .OrderByDescending(x => x.TotalCount)
            .ToList();

        // Issues by severity
        var issuesBySeverity = allIssues
            .GroupBy(i => i.Severity.ToString())
            .Select(g => new IssueBySeverityDto
            {
                Severity = g.Key,
                TotalCount = g.Count(),
                OpenCount = g.Count(i => i.Status != IssueStatus.Resolved && i.Status != IssueStatus.Closed),
                ResolvedCount = g.Count(i => i.Status == IssueStatus.Resolved || i.Status == IssueStatus.Closed)
            })
            .OrderByDescending(x => x.TotalCount)
            .ToList();

        // Top sites with issues
        var issuesBySite = allVisits
            .Where(v => v.IssuesFound.Any())
            .GroupBy(v => v.SiteId)
            .Select(g => new
            {
                SiteId = g.Key,
                SiteCode = g.First().SiteCode,
                SiteName = g.First().SiteName,
                Issues = g.SelectMany(v => v.IssuesFound).ToList()
            })
            .Select(x => new IssueBySiteDto
            {
                SiteId = x.SiteId,
                SiteCode = x.SiteCode,
                SiteName = x.SiteName,
                TotalIssues = x.Issues.Count,
                OpenIssues = x.Issues.Count(i => i.Status != IssueStatus.Resolved && i.Status != IssueStatus.Closed),
                CriticalIssues = x.Issues.Count(i => i.Severity == IssueSeverity.Critical)
            })
            .OrderByDescending(x => x.TotalIssues)
            .Take(10)
            .ToList();

        var report = new IssueAnalyticsReportDto
        {
            TotalIssues = totalIssues,
            OpenIssues = openIssues,
            ResolvedIssues = resolvedIssues,
            CriticalIssues = criticalIssues,
            HighIssues = highIssues,
            MediumIssues = mediumIssues,
            LowIssues = lowIssues,
            ResolutionRate = Math.Round(resolutionRate, 2),
            AverageResolutionTime = Math.Round((decimal)avgResolutionTime, 2),
            IssuesByCategory = issuesByCategory,
            IssuesBySeverity = issuesBySeverity,
            TopSitesWithIssues = issuesBySite,
            FromDate = fromDate,
            ToDate = toDate
        };

        return Result.Success(report);
    }
}

