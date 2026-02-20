namespace TelecomPM.Application.Queries.Kpi.GetOperationsDashboard;

using MediatR;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Kpi;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Interfaces.Repositories;
using TelecomPM.Domain.Specifications.VisitSpecifications;
using TelecomPM.Domain.Specifications.WorkOrderSpecifications;

public sealed class GetOperationsDashboardQueryHandler : IRequestHandler<GetOperationsDashboardQuery, Result<OperationsKpiDashboardDto>>
{
    private readonly IWorkOrderRepository _workOrderRepository;
    private readonly IVisitRepository _visitRepository;

    public GetOperationsDashboardQueryHandler(
        IWorkOrderRepository workOrderRepository,
        IVisitRepository visitRepository)
    {
        _workOrderRepository = workOrderRepository;
        _visitRepository = visitRepository;
    }

    public async Task<Result<OperationsKpiDashboardDto>> Handle(GetOperationsDashboardQuery request, CancellationToken cancellationToken)
    {
        var nowUtc = DateTime.UtcNow;

        var totalWorkOrders = await _workOrderRepository.CountAsync(
            new OperationsDashboardWorkOrdersSpecification(
                request.OfficeCode,
                request.SlaClass,
                request.FromDateUtc,
                request.ToDateUtc),
            cancellationToken);

        var openWorkOrders = await _workOrderRepository.CountAsync(
            new OperationsDashboardWorkOrdersSpecification(
                request.OfficeCode,
                request.SlaClass,
                request.FromDateUtc,
                request.ToDateUtc,
                onlyOpen: true),
            cancellationToken);

        var breachedWorkOrders = await _workOrderRepository.CountAsync(
            new OperationsDashboardWorkOrdersSpecification(
                request.OfficeCode,
                request.SlaClass,
                request.FromDateUtc,
                request.ToDateUtc,
                onlyBreached: true,
                nowUtc: nowUtc),
            cancellationToken);

        var atRiskWorkOrders = await _workOrderRepository.CountAsync(
            new OperationsDashboardWorkOrdersSpecification(
                request.OfficeCode,
                request.SlaClass,
                request.FromDateUtc,
                request.ToDateUtc,
                onlyAtRisk: true,
                nowUtc: nowUtc),
            cancellationToken);

        var reviewedVisitsCount = await _visitRepository.CountAsync(
            new OperationsDashboardVisitsSpecification(
                request.FromDateUtc,
                request.ToDateUtc,
                reviewedOnly: true),
            cancellationToken);

        var totalSubmittedVisits = await _visitRepository.CountAsync(
            new OperationsDashboardVisitsSpecification(
                request.FromDateUtc,
                request.ToDateUtc,
                submittedOnly: true),
            cancellationToken);

        var evidenceCompleteVisits = await _visitRepository.CountAsync(
            new OperationsDashboardVisitsSpecification(
                request.FromDateUtc,
                request.ToDateUtc,
                submittedOnly: true,
                evidenceCompleteOnly: true),
            cancellationToken);

        var closedWorkOrders = await _workOrderRepository.CountClosedAsync(
            request.OfficeCode,
            request.SlaClass,
            request.FromDateUtc,
            request.ToDateUtc,
            cancellationToken);

        var closedWithReworkOrReopened = await _workOrderRepository.CountClosedWithReworkOrReopenedHistoryAsync(
            request.OfficeCode,
            request.SlaClass,
            request.FromDateUtc,
            request.ToDateUtc,
            cancellationToken);

        var reopenedClosedWorkOrders = await _workOrderRepository.CountClosedWithReopenedHistoryAsync(
            request.OfficeCode,
            request.SlaClass,
            request.FromDateUtc,
            request.ToDateUtc,
            cancellationToken);

        var ftfRatePercent = Percentage(
            Math.Max(closedWorkOrders - closedWithReworkOrReopened, 0),
            Math.Max(closedWorkOrders, 1));

        var reopenRatePercent = Percentage(reopenedClosedWorkOrders, Math.Max(closedWorkOrders, 1));

        var mttrHours = await _workOrderRepository.GetClosedMeanTimeToRepairHoursAsync(
            request.OfficeCode,
            request.SlaClass,
            request.FromDateUtc,
            request.ToDateUtc,
            cancellationToken);

        var evidenceCompletenessPercent = Percentage(evidenceCompleteVisits, Math.Max(totalSubmittedVisits, 1));

        var dto = new OperationsKpiDashboardDto
        {
            GeneratedAtUtc = nowUtc,
            FromDateUtc = request.FromDateUtc,
            ToDateUtc = request.ToDateUtc,
            OfficeCode = request.OfficeCode,
            SlaClass = request.SlaClass,
            TotalWorkOrders = totalWorkOrders,
            OpenWorkOrders = openWorkOrders,
            BreachedWorkOrders = breachedWorkOrders,
            AtRiskWorkOrders = atRiskWorkOrders,
            SlaCompliancePercentage = Percentage(closedWorkOrders - breachedWorkOrders, Math.Max(closedWorkOrders, 1)),
            TotalReviewedVisits = reviewedVisitsCount,
            FtfRatePercent = ftfRatePercent,
            MttrHours = mttrHours,
            ReopenRatePercent = reopenRatePercent,
            EvidenceCompletenessPercent = evidenceCompletenessPercent,
            // Backward-compatible aliases
            FirstTimeFixPercentage = ftfRatePercent,
            ReopenRatePercentage = reopenRatePercent,
            EvidenceCompletenessPercentage = evidenceCompletenessPercent,
            MeanTimeToRepairHours = mttrHours
        };

        return Result.Success(dto);
    }

    private static decimal Percentage(int numerator, int denominator)
    {
        if (denominator <= 0)
            return 0;

        return Math.Round((decimal)numerator / denominator * 100m, 2);
    }
}
