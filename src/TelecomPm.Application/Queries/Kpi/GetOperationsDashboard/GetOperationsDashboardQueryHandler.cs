namespace TelecomPM.Application.Queries.Kpi.GetOperationsDashboard;

using MediatR;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Kpi;
using TelecomPM.Domain.Entities.Visits;
using TelecomPM.Domain.Entities.WorkOrders;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Interfaces.Repositories;

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

        var workOrders = await _workOrderRepository.GetAllAsNoTrackingAsync(cancellationToken);
        var filteredWorkOrders = workOrders
            .Where(wo => string.IsNullOrWhiteSpace(request.OfficeCode) || wo.OfficeCode.Equals(request.OfficeCode, StringComparison.OrdinalIgnoreCase))
            .Where(wo => !request.SlaClass.HasValue || wo.SlaClass == request.SlaClass.Value)
            .Where(wo => !request.FromDateUtc.HasValue || wo.CreatedAt >= request.FromDateUtc.Value)
            .Where(wo => !request.ToDateUtc.HasValue || wo.CreatedAt <= request.ToDateUtc.Value)
            .ToList();

        var visits = await _visitRepository.GetAllAsNoTrackingAsync(cancellationToken);
        var filteredVisits = visits
            .Where(v => !request.FromDateUtc.HasValue || v.CreatedAt >= request.FromDateUtc.Value)
            .Where(v => !request.ToDateUtc.HasValue || v.CreatedAt <= request.ToDateUtc.Value)
            .ToList();

        var breachedWorkOrders = filteredWorkOrders.Count(wo => IsBreached(wo, nowUtc));
        var atRiskWorkOrders = filteredWorkOrders.Count(wo => IsAtRisk(wo, nowUtc));
        var openWorkOrders = filteredWorkOrders.Count(wo => wo.Status is not WorkOrderStatus.Closed and not WorkOrderStatus.Cancelled);
        var closedWorkOrders = filteredWorkOrders.Count - openWorkOrders;

        var reviewedVisits = filteredVisits.Where(v => v.Status is VisitStatus.Approved or VisitStatus.Rejected).ToList();
        var approvedVisits = reviewedVisits.Count(v => v.Status == VisitStatus.Approved);
        var rejectedVisits = reviewedVisits.Count(v => v.Status == VisitStatus.Rejected);

        var visitsWithCorrections = filteredVisits.Count(v => v.ApprovalHistory.Any(h => h.Action == ApprovalAction.RequestCorrection));
        var evidenceCompleteVisits = filteredVisits.Count(v => v.IsReadingsComplete && v.IsPhotosComplete && v.IsChecklistComplete);

        var mttrSamples = filteredVisits
            .Where(v => v.Status == VisitStatus.Approved && v.ActualDuration is not null)
            .Select(v => v.ActualDuration!.Duration.TotalHours)
            .ToList();

        var dto = new OperationsKpiDashboardDto
        {
            GeneratedAtUtc = nowUtc,
            FromDateUtc = request.FromDateUtc,
            ToDateUtc = request.ToDateUtc,
            OfficeCode = request.OfficeCode,
            SlaClass = request.SlaClass,
            TotalWorkOrders = filteredWorkOrders.Count,
            OpenWorkOrders = openWorkOrders,
            BreachedWorkOrders = breachedWorkOrders,
            AtRiskWorkOrders = atRiskWorkOrders,
            SlaCompliancePercentage = Percentage(closedWorkOrders - breachedWorkOrders, Math.Max(closedWorkOrders, 1)),
            TotalReviewedVisits = reviewedVisits.Count,
            FirstTimeFixPercentage = Percentage(Math.Max(approvedVisits - visitsWithCorrections, 0), Math.Max(reviewedVisits.Count, 1)),
            ReopenRatePercentage = Percentage(rejectedVisits, Math.Max(reviewedVisits.Count, 1)),
            EvidenceCompletenessPercentage = Percentage(evidenceCompleteVisits, Math.Max(filteredVisits.Count, 1)),
            MeanTimeToRepairHours = mttrSamples.Count == 0 ? 0 : (decimal)mttrSamples.Average()
        };

        return Result.Success(dto);
    }

    private static bool IsBreached(WorkOrder workOrder, DateTime nowUtc)
    {
        if (workOrder.Status is WorkOrderStatus.Closed or WorkOrderStatus.Cancelled)
            return false;

        return nowUtc > workOrder.ResolutionDeadlineUtc;
    }

    private static bool IsAtRisk(WorkOrder workOrder, DateTime nowUtc)
    {
        if (workOrder.Status is WorkOrderStatus.Closed or WorkOrderStatus.Cancelled)
            return false;

        return nowUtc <= workOrder.ResolutionDeadlineUtc && nowUtc >= workOrder.ResolutionDeadlineUtc.AddHours(-2);
    }

    private static decimal Percentage(int numerator, int denominator)
    {
        if (denominator <= 0)
            return 0;

        return Math.Round((decimal)numerator / denominator * 100m, 2);
    }
}
