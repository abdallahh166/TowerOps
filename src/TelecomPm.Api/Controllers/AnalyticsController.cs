namespace TelecomPm.Api.Controllers;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TelecomPM.Application.Queries.Reports.GetEngineerPerformanceReport;
using TelecomPM.Application.Queries.Reports.GetSiteMaintenanceReport;
using TelecomPM.Application.Queries.Reports.GetOfficeStatisticsReport;
using TelecomPM.Application.Queries.Reports.GetMaterialUsageSummary;
using TelecomPM.Application.Queries.Reports.GetVisitCompletionTrends;
using TelecomPM.Application.Queries.Reports.GetIssueAnalyticsReport;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class AnalyticsController : ApiControllerBase
{
    [HttpGet("engineer-performance/{engineerId:guid}")]
    public async Task<IActionResult> GetEngineerPerformance(
        Guid engineerId,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        CancellationToken cancellationToken)
    {
        var query = new GetEngineerPerformanceReportQuery
        {
            EngineerId = engineerId,
            FromDate = fromDate,
            ToDate = toDate
        };

        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("site-maintenance/{siteId:guid}")]
    public async Task<IActionResult> GetSiteMaintenance(
        Guid siteId,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        CancellationToken cancellationToken)
    {
        var query = new GetSiteMaintenanceReportQuery
        {
            SiteId = siteId,
            FromDate = fromDate,
            ToDate = toDate
        };

        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("office-statistics/{officeId:guid}")]
    public async Task<IActionResult> GetOfficeStatistics(
        Guid officeId,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        CancellationToken cancellationToken)
    {
        var query = new GetOfficeStatisticsReportQuery
        {
            OfficeId = officeId,
            FromDate = fromDate,
            ToDate = toDate
        };

        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("material-usage/{materialId:guid}")]
    public async Task<IActionResult> GetMaterialUsage(
        Guid materialId,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        CancellationToken cancellationToken)
    {
        var query = new GetMaterialUsageSummaryQuery
        {
            MaterialId = materialId,
            FromDate = fromDate,
            ToDate = toDate
        };

        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("visit-completion-trends")]
    public async Task<IActionResult> GetVisitCompletionTrends(
        [FromQuery] Guid? officeId,
        [FromQuery] Guid? engineerId,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] string? period,
        CancellationToken cancellationToken)
    {
        var periodValue = period ?? "Monthly";
        if (!Enum.TryParse<TrendPeriod>(periodValue, true, out var trendPeriod))
        {
            trendPeriod = TrendPeriod.Monthly;
        }

        var query = new GetVisitCompletionTrendsQuery
        {
            OfficeId = officeId,
            EngineerId = engineerId,
            FromDate = fromDate ?? DateTime.UtcNow.AddMonths(-3),
            ToDate = toDate ?? DateTime.UtcNow,
            Period = trendPeriod
        };

        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("issue-analytics")]
    public async Task<IActionResult> GetIssueAnalytics(
        [FromQuery] Guid? officeId,
        [FromQuery] Guid? siteId,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        CancellationToken cancellationToken)
    {
        var query = new GetIssueAnalyticsReportQuery
        {
            OfficeId = officeId,
            SiteId = siteId,
            FromDate = fromDate,
            ToDate = toDate
        };

        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }
}

