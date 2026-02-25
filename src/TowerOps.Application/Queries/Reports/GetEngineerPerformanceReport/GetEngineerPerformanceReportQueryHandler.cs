namespace TowerOps.Application.Queries.Reports.GetEngineerPerformanceReport;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Reports;
using TowerOps.Domain.Enums;
using TowerOps.Domain.Interfaces.Repositories;
using TowerOps.Domain.Specifications.VisitSpecifications;

public class GetEngineerPerformanceReportQueryHandler 
    : IRequestHandler<GetEngineerPerformanceReportQuery, Result<EngineerPerformanceReportDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IVisitRepository _visitRepository;
    private readonly IOfficeRepository _officeRepository;

    public GetEngineerPerformanceReportQueryHandler(
        IUserRepository userRepository,
        IVisitRepository visitRepository,
        IOfficeRepository officeRepository)
    {
        _userRepository = userRepository;
        _visitRepository = visitRepository;
        _officeRepository = officeRepository;
    }

    public async Task<Result<EngineerPerformanceReportDto>> Handle(
        GetEngineerPerformanceReportQuery request,
        CancellationToken cancellationToken)
    {
        var engineer = await _userRepository.GetByIdAsync(request.EngineerId, cancellationToken);
        if (engineer == null)
            return Result.Failure<EngineerPerformanceReportDto>("Engineer not found");

        if (engineer.Role != UserRole.PMEngineer)
            return Result.Failure<EngineerPerformanceReportDto>("User is not a PM Engineer");

        var office = await _officeRepository.GetByIdAsync(engineer.OfficeId, cancellationToken);
        if (office == null)
            return Result.Failure<EngineerPerformanceReportDto>("Office not found");

        var fromDate = request.FromDate ?? DateTime.UtcNow.AddMonths(-3);
        var toDate = request.ToDate ?? DateTime.UtcNow;

        var visitSpec = new EngineerVisitsFilteredSpecification(
            request.EngineerId,
            fromUtc: fromDate,
            toUtc: toDate);
        var visits = await _visitRepository.FindAsNoTrackingAsync(visitSpec, cancellationToken);

        // Calculate metrics
        var totalVisits = visits.Count;
        var completedVisits = visits.Count(v => v.Status == VisitStatus.Completed);
        var approvedVisits = visits.Count(v => v.Status == VisitStatus.Approved);
        var rejectedVisits = visits.Count(v => v.Status == VisitStatus.Rejected);
        var pendingReviewVisits = visits.Count(v => v.Status == VisitStatus.Submitted || v.Status == VisitStatus.UnderReview);
        var onTimeVisits = visits.Count(v => v.Status == VisitStatus.Completed && v.ActualEndTime.HasValue && v.ScheduledDate.Date >= v.ActualEndTime.Value.Date);
        var overdueVisits = visits.Count(v => v.Status != VisitStatus.Completed && v.ScheduledDate < DateTime.UtcNow);

        var completionRate = totalVisits > 0 ? (decimal)completedVisits / totalVisits * 100 : 0;
        var approvalRate = completedVisits > 0 ? (decimal)approvedVisits / completedVisits * 100 : 0;
        var onTimeRate = completedVisits > 0 ? (decimal)onTimeVisits / completedVisits * 100 : 0;

        var visitsNeedingCorrection = visits.Count(v => v.Status == VisitStatus.NeedsCorrection);
        var criticalIssues = visits.SelectMany(v => v.IssuesFound).Count(i => i.Severity == IssueSeverity.Critical);
        
        var averageDuration = visits
            .Where(v => v.ActualDuration != null)
            .Select(v => v.ActualDuration!.Duration.TotalHours)
            .DefaultIfEmpty(0)
            .Average();

        var report = new EngineerPerformanceReportDto
        {
            EngineerId = engineer.Id,
            EngineerName = engineer.Name,
            EngineerEmail = engineer.Email,
            OfficeId = office.Id,
            OfficeName = office.Name,
            Specializations = engineer.Specializations.ToList(),
            AssignedSitesCount = engineer.AssignedSiteIds.Count,
            MaxAssignedSites = engineer.MaxAssignedSites,
            CapacityUtilization = engineer.MaxAssignedSites.HasValue && engineer.MaxAssignedSites.Value > 0
                ? (int)((decimal)engineer.AssignedSiteIds.Count / engineer.MaxAssignedSites.Value * 100)
                : 0,
            TotalVisits = totalVisits,
            CompletedVisits = completedVisits,
            ApprovedVisits = approvedVisits,
            RejectedVisits = rejectedVisits,
            PendingReviewVisits = pendingReviewVisits,
            OnTimeVisits = onTimeVisits,
            OverdueVisits = overdueVisits,
            CompletionRate = Math.Round(completionRate, 2),
            ApprovalRate = Math.Round(approvalRate, 2),
            OnTimeRate = Math.Round(onTimeRate, 2),
            PerformanceRating = engineer.PerformanceRating,
            VisitsNeedingCorrection = visitsNeedingCorrection,
            CriticalIssuesReported = criticalIssues,
            AverageVisitDuration = Math.Round((decimal)averageDuration, 2),
            FromDate = fromDate,
            ToDate = toDate
        };

        return Result.Success(report);
    }
}

