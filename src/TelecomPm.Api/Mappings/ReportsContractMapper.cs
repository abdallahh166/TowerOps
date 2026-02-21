namespace TelecomPm.Api.Mappings;

using TelecomPM.Application.Commands.Reports.GenerateContractorScorecard;
using TelecomPM.Application.Commands.Reports.ExportBDT;
using TelecomPM.Application.Commands.Reports.ExportChecklist;
using TelecomPM.Application.Commands.Reports.ExportDataCollection;
using TelecomPM.Application.Commands.Reports.ExportScorecard;
using TelecomPM.Domain.Enums;
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

    public static ExportChecklistCommand ToExportChecklistCommand(Guid? visitId, VisitType? visitType)
        => new()
        {
            VisitId = visitId,
            VisitType = visitType
        };

    public static ExportBDTCommand ToExportBdtCommand(DateTime? fromDateUtc, DateTime? toDateUtc)
        => new()
        {
            FromDateUtc = fromDateUtc,
            ToDateUtc = toDateUtc
        };

    public static ExportDataCollectionCommand ToExportDataCollectionCommand(string? officeCode)
        => new()
        {
            OfficeCode = officeCode
        };

    public static ExportScorecardCommand ToExportScorecardCommand(string officeCode, int month, int year)
        => new()
        {
            OfficeCode = officeCode,
            Month = month,
            Year = year
        };
}
