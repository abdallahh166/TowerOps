namespace TowerOps.Api.Mappings;

using TowerOps.Application.Commands.Reports.GenerateContractorScorecard;
using TowerOps.Application.Commands.Reports.ExportBDT;
using TowerOps.Application.Commands.Reports.ExportChecklist;
using TowerOps.Application.Commands.Reports.ExportDataCollection;
using TowerOps.Application.Commands.Reports.ExportScorecard;
using TowerOps.Domain.Enums;
using TowerOps.Application.Queries.Reports.GetVisitReport;

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
