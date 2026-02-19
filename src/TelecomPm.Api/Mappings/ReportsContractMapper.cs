namespace TelecomPm.Api.Mappings;

using TelecomPM.Application.Queries.Reports.GetVisitReport;

public static class ReportsContractMapper
{
    public static GetVisitReportQuery ToVisitReportQuery(this Guid visitId)
        => new() { VisitId = visitId };
}
