namespace TelecomPM.Application.Queries.Reports.GetOfficeStatisticsReport;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Reports;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Interfaces.Repositories;
using TelecomPM.Domain.Specifications.SiteSpecifications;
using TelecomPM.Domain.Specifications.VisitSpecifications;
using TelecomPM.Domain.Specifications.UserSpecifications;
using TelecomPM.Domain.Specifications.MaterialSpecifications;

public class GetOfficeStatisticsReportQueryHandler 
    : IRequestHandler<GetOfficeStatisticsReportQuery, Result<OfficeStatisticsReportDto>>
{
    private readonly IOfficeRepository _officeRepository;
    private readonly ISiteRepository _siteRepository;
    private readonly IUserRepository _userRepository;
    private readonly IVisitRepository _visitRepository;
    private readonly IMaterialRepository _materialRepository;

    public GetOfficeStatisticsReportQueryHandler(
        IOfficeRepository officeRepository,
        ISiteRepository siteRepository,
        IUserRepository userRepository,
        IVisitRepository visitRepository,
        IMaterialRepository materialRepository)
    {
        _officeRepository = officeRepository;
        _siteRepository = siteRepository;
        _userRepository = userRepository;
        _visitRepository = visitRepository;
        _materialRepository = materialRepository;
    }

    public async Task<Result<OfficeStatisticsReportDto>> Handle(
        GetOfficeStatisticsReportQuery request,
        CancellationToken cancellationToken)
    {
        var office = await _officeRepository.GetByIdAsNoTrackingAsync(request.OfficeId, cancellationToken);
        if (office == null)
            return Result.Failure<OfficeStatisticsReportDto>("Office not found");

        var fromDate = request.FromDate ?? DateTime.UtcNow.AddMonths(-3);
        var toDate = request.ToDate ?? DateTime.UtcNow;

        // Sites
        var siteSpec = new SitesByOfficeSpecification(request.OfficeId);
        var sites = await _siteRepository.FindAsNoTrackingAsync(siteSpec, cancellationToken);
        var totalSites = sites.Count;
        var activeSites = sites.Count(s => s.Status == SiteStatus.OnAir);
        var inactiveSites = totalSites - activeSites;
        var assignedSites = sites.Count(s => s.AssignedEngineerId.HasValue);
        var unassignedSites = totalSites - assignedSites;
        var sitesNeedingMaintenance = sites.Count(s => s.Status == SiteStatus.UnderMaintenance);

        // Users
        var userSpec = new UsersByOfficeSpecification(request.OfficeId);
        var users = await _userRepository.FindAsNoTrackingAsync(userSpec, cancellationToken);
        var totalUsers = users.Count;
        var engineers = users.Where(u => u.Role == UserRole.PMEngineer).ToList();
        var technicians = users.Where(u => u.Role == UserRole.Technician).ToList();
        var totalEngineers = engineers.Count;
        var activeEngineers = engineers.Count(u => u.IsActive);
        var totalTechnicians = technicians.Count;
        var activeTechnicians = technicians.Count(u => u.IsActive);

        // Visits
        var visitSpec = new VisitsByDateRangeSpecification(fromDate, toDate);
        var allVisits = await _visitRepository.FindAsNoTrackingAsync(visitSpec, cancellationToken);
        var officeVisits = allVisits.Where(v => sites.Any(s => s.Id == v.SiteId)).ToList();
        
        var totalVisits = officeVisits.Count;
        var scheduledVisits = officeVisits.Count(v => v.Status == VisitStatus.Scheduled);
        var completedVisits = officeVisits.Count(v => v.Status == VisitStatus.Completed);
        var approvedVisits = officeVisits.Count(v => v.Status == VisitStatus.Approved);
        var rejectedVisits = officeVisits.Count(v => v.Status == VisitStatus.Rejected);
        var pendingReviewVisits = officeVisits.Count(v => v.Status == VisitStatus.Submitted || v.Status == VisitStatus.UnderReview);
        var overdueVisits = officeVisits.Count(v => v.Status != VisitStatus.Completed && v.ScheduledDate < DateTime.UtcNow);

        // Materials
        var materialSpec = new MaterialsByOfficeSpecification(request.OfficeId);
        var materials = await _materialRepository.FindAsNoTrackingAsync(materialSpec, cancellationToken);
        var totalMaterials = materials.Count;
        var activeMaterials = materials.Count(m => m.IsActive);
        var lowStockMaterials = materials.Count(m => m.IsStockLow());
        var totalMaterialValue = materials.Sum(m => m.CurrentStock.Value * m.UnitCost.Amount);

        // Performance metrics
        var completionRate = totalVisits > 0 ? (decimal)completedVisits / totalVisits * 100 : 0;
        var approvalRate = completedVisits > 0 ? (decimal)approvedVisits / completedVisits * 100 : 0;
        var avgPerformanceRating = engineers.Where(e => e.PerformanceRating.HasValue)
            .Select(e => e.PerformanceRating!.Value)
            .DefaultIfEmpty(0)
            .Average();

        // Issues
        var allIssues = officeVisits.SelectMany(v => v.IssuesFound).ToList();
        var totalIssues = allIssues.Count;
        var openIssues = allIssues.Count(i => i.Status != IssueStatus.Resolved && i.Status != IssueStatus.Closed);
        var criticalIssues = allIssues.Count(i => i.Severity == IssueSeverity.Critical);
        var resolvedIssues = allIssues.Count(i => i.Status == IssueStatus.Resolved || i.Status == IssueStatus.Closed);

        // Top engineers
        var topEngineers = engineers
            .Where(e => e.AssignedSiteIds.Count > 0)
            .Select(e => new EngineerSummaryDto
            {
                EngineerId = e.Id,
                EngineerName = e.Name,
                AssignedSites = e.AssignedSiteIds.Count,
                CompletedVisits = officeVisits.Count(v => v.EngineerId == e.Id && v.Status == VisitStatus.Completed),
                CompletionRate = officeVisits.Any(v => v.EngineerId == e.Id)
                    ? (decimal)officeVisits.Count(v => v.EngineerId == e.Id && v.Status == VisitStatus.Completed) / officeVisits.Count(v => v.EngineerId == e.Id) * 100
                    : 0,
                PerformanceRating = e.PerformanceRating
            })
            .OrderByDescending(e => e.CompletionRate)
            .Take(5)
            .ToList();

        var report = new OfficeStatisticsReportDto
        {
            OfficeId = office.Id,
            OfficeCode = office.Code,
            OfficeName = office.Name,
            Region = office.Region,
            TotalSites = totalSites,
            ActiveSites = activeSites,
            InactiveSites = inactiveSites,
            AssignedSites = assignedSites,
            UnassignedSites = unassignedSites,
            SitesNeedingMaintenance = sitesNeedingMaintenance,
            TotalEngineers = totalEngineers,
            ActiveEngineers = activeEngineers,
            TotalTechnicians = totalTechnicians,
            ActiveTechnicians = activeTechnicians,
            TotalUsers = totalUsers,
            TotalVisits = totalVisits,
            ScheduledVisits = scheduledVisits,
            CompletedVisits = completedVisits,
            ApprovedVisits = approvedVisits,
            RejectedVisits = rejectedVisits,
            PendingReviewVisits = pendingReviewVisits,
            OverdueVisits = overdueVisits,
            TotalMaterials = totalMaterials,
            ActiveMaterials = activeMaterials,
            LowStockMaterials = lowStockMaterials,
            TotalMaterialValue = totalMaterialValue,
            AverageVisitCompletionRate = Math.Round(completionRate, 2),
            AverageVisitApprovalRate = Math.Round(approvalRate, 2),
            AverageEngineerPerformance = Math.Round((decimal)avgPerformanceRating, 2),
            TotalIssues = totalIssues,
            OpenIssues = openIssues,
            CriticalIssues = criticalIssues,
            ResolvedIssues = resolvedIssues,
            TopEngineers = topEngineers,
            FromDate = fromDate,
            ToDate = toDate
        };

        return Result.Success(report);
    }
}

