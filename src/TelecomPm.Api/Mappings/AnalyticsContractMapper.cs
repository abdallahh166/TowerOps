namespace TelecomPm.Api.Mappings;

using TelecomPM.Application.Queries.Reports.GetEngineerPerformanceReport;
using TelecomPM.Application.Queries.Reports.GetIssueAnalyticsReport;
using TelecomPM.Application.Queries.Reports.GetMaterialUsageSummary;
using TelecomPM.Application.Queries.Reports.GetOfficeStatisticsReport;
using TelecomPM.Application.Queries.Reports.GetSiteMaintenanceReport;
using TelecomPM.Application.Queries.Reports.GetVisitCompletionTrends;

public static class AnalyticsContractMapper
{
    public static GetEngineerPerformanceReportQuery ToEngineerPerformanceQuery(this Guid engineerId, DateTime? fromDate, DateTime? toDate)
        => new() { EngineerId = engineerId, FromDate = fromDate, ToDate = toDate };

    public static GetSiteMaintenanceReportQuery ToSiteMaintenanceQuery(this Guid siteId, DateTime? fromDate, DateTime? toDate)
        => new() { SiteId = siteId, FromDate = fromDate, ToDate = toDate };

    public static GetOfficeStatisticsReportQuery ToOfficeStatisticsQuery(this Guid officeId, DateTime? fromDate, DateTime? toDate)
        => new() { OfficeId = officeId, FromDate = fromDate, ToDate = toDate };

    public static GetMaterialUsageSummaryQuery ToMaterialUsageQuery(this Guid materialId, DateTime? fromDate, DateTime? toDate)
        => new() { MaterialId = materialId, FromDate = fromDate, ToDate = toDate };

    public static GetVisitCompletionTrendsQuery ToVisitCompletionTrendsQuery(
        Guid? officeId,
        Guid? engineerId,
        DateTime? fromDate,
        DateTime? toDate,
        TrendPeriod period)
        => new()
        {
            OfficeId = officeId,
            EngineerId = engineerId,
            FromDate = fromDate ?? DateTime.UtcNow.AddMonths(-3),
            ToDate = toDate ?? DateTime.UtcNow,
            Period = period
        };

    public static GetIssueAnalyticsReportQuery ToIssueAnalyticsQuery(this Guid? officeId, Guid? siteId, DateTime? fromDate, DateTime? toDate)
        => new() { OfficeId = officeId, SiteId = siteId, FromDate = fromDate, ToDate = toDate };
}
