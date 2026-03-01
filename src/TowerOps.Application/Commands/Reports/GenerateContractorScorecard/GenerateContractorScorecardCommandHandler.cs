using ClosedXML.Excel;
using MediatR;
using TowerOps.Application.Common;
using TowerOps.Application.Queries.Kpi.GetOperationsDashboard;
using TowerOps.Domain.Enums;
using TowerOps.Domain.Interfaces.Repositories;
using TowerOps.Domain.Specifications.VisitSpecifications;
using TowerOps.Domain.Specifications.WorkOrderSpecifications;

namespace TowerOps.Application.Commands.Reports.GenerateContractorScorecard;

public class GenerateContractorScorecardCommandHandler : IRequestHandler<GenerateContractorScorecardCommand, Result<byte[]>>
{
    private readonly IOfficeRepository _officeRepository;
    private readonly ISiteRepository _siteRepository;
    private readonly IWorkOrderRepository _workOrderRepository;
    private readonly IVisitRepository _visitRepository;
    private readonly ISender _sender;

    public GenerateContractorScorecardCommandHandler(
        IOfficeRepository officeRepository,
        ISiteRepository siteRepository,
        IWorkOrderRepository workOrderRepository,
        IVisitRepository visitRepository,
        ISender sender)
    {
        _officeRepository = officeRepository;
        _siteRepository = siteRepository;
        _workOrderRepository = workOrderRepository;
        _visitRepository = visitRepository;
        _sender = sender;
    }

    public async Task<Result<byte[]>> Handle(GenerateContractorScorecardCommand request, CancellationToken cancellationToken)
    {
        var fromUtc = new DateTime(request.Year, request.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var toUtc = fromUtc.AddMonths(1).AddTicks(-1);

        var office = await _officeRepository.GetByCodeAsNoTrackingAsync(request.OfficeCode, cancellationToken);
        if (office is null)
            return Result.Failure<byte[]>($"Office code '{request.OfficeCode}' not found.");

        var sites = await _siteRepository.GetByOfficeIdAsNoTrackingAsync(office.Id, cancellationToken);
        var siteCodes = sites.Select(s => s.SiteCode.Value).ToHashSet(StringComparer.OrdinalIgnoreCase);

        var workOrders = await _workOrderRepository.FindAsNoTrackingAsync(
            new ContractorScorecardWorkOrdersSpecification(request.OfficeCode, fromUtc, toUtc),
            cancellationToken);

        var visits = siteCodes.Count == 0
            ? new List<Domain.Entities.Visits.Visit>()
            : (await _visitRepository.FindAsNoTrackingAsync(
                new ContractorScorecardVisitsSpecification(siteCodes, fromUtc, toUtc),
                cancellationToken)).ToList();

        var kpiResult = await _sender.Send(new GetOperationsDashboardQuery
        {
            OfficeCode = request.OfficeCode,
            FromDateUtc = fromUtc,
            ToDateUtc = toUtc
        }, cancellationToken);

        if (kpiResult.IsFailure || kpiResult.Value is null)
            return Result.Failure<byte[]>($"Failed to calculate KPIs: {kpiResult.Error}");

        var totalWosCreated = workOrders.Count;
        var totalWosClosed = workOrders.Count(wo => wo.Status == WorkOrderStatus.Closed);
        var totalMaterialsConsumed = visits.Sum(v => v.MaterialsUsed.Count);

        using var workbook = new XLWorkbook();
        BuildSummarySheet(
            workbook,
            request.OfficeCode,
            request.Month,
            request.Year,
            totalWosCreated,
            totalWosClosed,
            totalMaterialsConsumed,
            kpiResult.Value);

        BuildWorkOrderDetailSheet(workbook, workOrders);
        BuildEvidenceDetailSheet(workbook, visits);

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return Result.Success(stream.ToArray());
    }

    private static void BuildSummarySheet(
        XLWorkbook workbook,
        string officeCode,
        int month,
        int year,
        int totalWosCreated,
        int totalWosClosed,
        int totalMaterialsConsumed,
        DTOs.Kpi.OperationsKpiDashboardDto kpi)
    {
        var ws = workbook.AddWorksheet("Summary");
        ws.Cell(1, 1).Value = "Office";
        ws.Cell(1, 2).Value = officeCode;
        ws.Cell(2, 1).Value = "Month";
        ws.Cell(2, 2).Value = month;
        ws.Cell(3, 1).Value = "Year";
        ws.Cell(3, 2).Value = year;
        ws.Cell(4, 1).Value = "Total WOs Created";
        ws.Cell(4, 2).Value = totalWosCreated;
        ws.Cell(5, 1).Value = "Total WOs Closed";
        ws.Cell(5, 2).Value = totalWosClosed;
        ws.Cell(6, 1).Value = "SLA Compliance %";
        ws.Cell(6, 2).Value = kpi.SlaCompliancePercentage;
        ws.Cell(7, 1).Value = "FTF Rate %";
        ws.Cell(7, 2).Value = kpi.FtfRatePercent;
        ws.Cell(8, 1).Value = "MTTR Hours";
        ws.Cell(8, 2).Value = kpi.MttrHours;
        ws.Cell(9, 1).Value = "Reopen Rate %";
        ws.Cell(9, 2).Value = kpi.ReopenRatePercent;
        ws.Cell(10, 1).Value = "Evidence Completeness %";
        ws.Cell(10, 2).Value = kpi.EvidenceCompletenessPercent;
        ws.Cell(11, 1).Value = "Total Materials Consumed";
        ws.Cell(11, 2).Value = totalMaterialsConsumed;
        ws.Columns().AdjustToContents();
    }

    private static void BuildWorkOrderDetailSheet(XLWorkbook workbook, IReadOnlyList<Domain.Entities.WorkOrders.WorkOrder> workOrders)
    {
        var ws = workbook.AddWorksheet("WO Details");
        ws.Cell(1, 1).Value = "WoNumber";
        ws.Cell(1, 2).Value = "SiteCode";
        ws.Cell(1, 3).Value = "CreatedAt";
        ws.Cell(1, 4).Value = "ClosedAt";
        ws.Cell(1, 5).Value = "SlaClass";
        ws.Cell(1, 6).Value = "Status";
        ws.Cell(1, 7).Value = "SlaBreached";

        var row = 2;
        foreach (var wo in workOrders)
        {
            var closedAt = wo.Status == WorkOrderStatus.Closed ? wo.UpdatedAt : null;
            var slaBreached = closedAt.HasValue && closedAt.Value > wo.ResolutionDeadlineUtc;

            ws.Cell(row, 1).Value = wo.WoNumber;
            ws.Cell(row, 2).Value = wo.SiteCode;
            ws.Cell(row, 3).Value = wo.CreatedAt;
            ws.Cell(row, 4).Value = closedAt;
            ws.Cell(row, 5).Value = wo.SlaClass.ToString();
            ws.Cell(row, 6).Value = wo.Status.ToString();
            ws.Cell(row, 7).Value = slaBreached;
            row++;
        }

        ws.Columns().AdjustToContents();
    }

    private static void BuildEvidenceDetailSheet(XLWorkbook workbook, IReadOnlyList<Domain.Entities.Visits.Visit> visits)
    {
        var ws = workbook.AddWorksheet("Evidence Details");
        ws.Cell(1, 1).Value = "VisitNumber";
        ws.Cell(1, 2).Value = "SiteCode";
        ws.Cell(1, 3).Value = "Engineer";
        ws.Cell(1, 4).Value = "PhotosCount";
        ws.Cell(1, 5).Value = "HasReadings";
        ws.Cell(1, 6).Value = "ChecklistPercent";

        var row = 2;
        foreach (var visit in visits)
        {
            var checklistPercent = visit.Checklists.Count == 0
                ? 0
                : visit.Checklists.Count(c => c.Status != CheckStatus.NA) * 100 / visit.Checklists.Count;

            ws.Cell(row, 1).Value = visit.VisitNumber;
            ws.Cell(row, 2).Value = visit.SiteCode;
            ws.Cell(row, 3).Value = visit.EngineerName;
            ws.Cell(row, 4).Value = visit.Photos.Count(p => p.FileStatus == UploadedFileStatus.Approved);
            ws.Cell(row, 5).Value = visit.Readings.Any();
            ws.Cell(row, 6).Value = checklistPercent;
            row++;
        }

        ws.Columns().AdjustToContents();
    }
}
