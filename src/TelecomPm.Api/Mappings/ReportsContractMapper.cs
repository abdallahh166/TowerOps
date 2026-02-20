namespace TelecomPm.Api.Mappings;

using TelecomPM.Application.Commands.Reports.GenerateContractorScorecard;
using TelecomPM.Application.Queries.Reports.GetVisitReport;

public static class ReportsContractMapper
{
    public static GetVisitReportQuery ToVisitReportQuery(this Guid visitId)
        => new() { VisitId = visitId };

    public static GenerateContractorScorecardCommand ToGenerateContractorScorecardCommand(string officeCode, int month, int year)
        => new()
        {
            OfficeCode = officeCode,
            Month = month,
            Year = year
        };
}
