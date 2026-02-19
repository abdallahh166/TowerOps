namespace TelecomPm.Api.Controllers;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TelecomPm.Api.Mappings;
using TelecomPM.Application.Queries.Reports.GetVisitCompletionTrends;

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
        var result = await Mediator.Send(engineerId.ToEngineerPerformanceQuery(fromDate, toDate), cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("site-maintenance/{siteId:guid}")]
    public async Task<IActionResult> GetSiteMaintenance(
        Guid siteId,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(siteId.ToSiteMaintenanceQuery(fromDate, toDate), cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("office-statistics/{officeId:guid}")]
    public async Task<IActionResult> GetOfficeStatistics(
        Guid officeId,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(officeId.ToOfficeStatisticsQuery(fromDate, toDate), cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("material-usage/{materialId:guid}")]
    public async Task<IActionResult> GetMaterialUsage(
        Guid materialId,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(materialId.ToMaterialUsageQuery(fromDate, toDate), cancellationToken);
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

        var query = AnalyticsContractMapper.ToVisitCompletionTrendsQuery(officeId, engineerId, fromDate, toDate, trendPeriod);
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
        var result = await Mediator.Send(officeId.ToIssueAnalyticsQuery(siteId, fromDate, toDate), cancellationToken);
        return HandleResult(result);
    }
}
