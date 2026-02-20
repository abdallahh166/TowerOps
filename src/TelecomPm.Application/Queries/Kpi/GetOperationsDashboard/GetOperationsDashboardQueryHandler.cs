namespace TelecomPM.Application.Queries.Kpi.GetOperationsDashboard;

using MediatR;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Kpi;
using TelecomPM.Domain.Entities.Visits;
using TelecomPM.Domain.Entities.WorkOrders;
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

        var filteredVisitsCount = await _visitRepository.CountAsync(
            new OperationsDashboardVisitsSpecification(request.FromDateUtc, request.ToDateUtc),
            cancellationToken);

        var reviewedVisitsCount = await _visitRepository.CountAsync(
            new OperationsDashboardVisitsSpecification(
                request.FromDateUtc,
                request.ToDateUtc,
                reviewedOnly: true),
            cancellationToken);

        var rejectedVisits = await _visitRepository.CountAsync(
            new OperationsDashboardVisitsSpecification(
                request.FromDateUtc,
                request.ToDateUtc,
                rejectedOnly: true),
            cancellationToken);

        var approvedVisitsWithDuration = await _visitRepository.FindAsNoTrackingAsync(
            new OperationsDashboardVisitsSpecification(
                request.FromDateUtc,
                request.ToDateUtc,
                approvedWithDurationOnly: true),
            cancellationToken);

        var approvedVisits = approvedVisitsWithDuration.Count;

        var visitsWithCorrections = await _visitRepository.CountAsync(
            new OperationsDashboardVisitsSpecification(
                request.FromDateUtc,
                request.ToDateUtc,
                withCorrectionsOnly: true),
            cancellationToken);

        var evidenceCompleteVisits = await _visitRepository.CountAsync(
            new OperationsDashboardVisitsSpecification(
                request.FromDateUtc,
                request.ToDateUtc,
                evidenceCompleteOnly: true),
            cancellationToken);

        var mttrSamples = approvedVisitsWithDuration
            .Select(v => v.ActualDuration!.Duration.TotalHours)
            .ToList();

        var closedWorkOrders = totalWorkOrders - openWorkOrders;

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
            FirstTimeFixPercentage = Percentage(Math.Max(approvedVisits - visitsWithCorrections, 0), Math.Max(reviewedVisitsCount, 1)),
            ReopenRatePercentage = Percentage(rejectedVisits, Math.Max(reviewedVisitsCount, 1)),
            EvidenceCompletenessPercentage = Percentage(evidenceCompleteVisits, Math.Max(filteredVisitsCount, 1)),
            MeanTimeToRepairHours = mttrSamples.Count == 0 ? 0 : (decimal)mttrSamples.Average()
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
