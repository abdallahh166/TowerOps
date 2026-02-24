namespace TelecomPM.Application.Queries.Reports.GetSiteMaintenanceReport;

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
using TelecomPM.Domain.Entities.Users;

public class GetSiteMaintenanceReportQueryHandler 
    : IRequestHandler<GetSiteMaintenanceReportQuery, Result<SiteMaintenanceReportDto>>
{
    private readonly ISiteRepository _siteRepository;
    private readonly IVisitRepository _visitRepository;
    private readonly IOfficeRepository _officeRepository;
    private readonly IUserRepository _userRepository;

    public GetSiteMaintenanceReportQueryHandler(
        ISiteRepository siteRepository,
        IVisitRepository visitRepository,
        IOfficeRepository officeRepository,
        IUserRepository userRepository)
    {
        _siteRepository = siteRepository;
        _visitRepository = visitRepository;
        _officeRepository = officeRepository;
        _userRepository = userRepository;
    }

    public async Task<Result<SiteMaintenanceReportDto>> Handle(
        GetSiteMaintenanceReportQuery request,
        CancellationToken cancellationToken)
    {
        var siteSpec = new SiteWithFullDetailsSpecification(request.SiteId);
        var site = await _siteRepository.FindOneAsNoTrackingAsync(siteSpec, cancellationToken);
        if (site == null)
            return Result.Failure<SiteMaintenanceReportDto>("Site not found");

        var office = await _officeRepository.GetByIdAsNoTrackingAsync(site.OfficeId, cancellationToken);
        if (office == null)
            return Result.Failure<SiteMaintenanceReportDto>("Office not found");

        var fromDate = request.FromDate ?? DateTime.UtcNow.AddMonths(-6);
        var toDate = request.ToDate ?? DateTime.UtcNow;

        var visitSpec = new VisitsBySiteSpecification(request.SiteId, fromDate, toDate);
        var visits = await _visitRepository.FindAsNoTrackingAsync(visitSpec, cancellationToken);

        var totalVisits = visits.Count;
        var completedVisits = visits.Count(v => v.Status == VisitStatus.Completed);
        
        var lastVisit = visits
            .Where(v => v.Status == VisitStatus.Completed && v.ActualEndTime.HasValue)
            .OrderByDescending(v => v.ActualEndTime)
            .FirstOrDefault();

        var nextScheduledVisit = visits
            .Where(v => v.Status == VisitStatus.Scheduled)
            .OrderBy(v => v.ScheduledDate)
            .FirstOrDefault();

        // Calculate issues
        var allIssues = visits.SelectMany(v => v.IssuesFound).ToList();
        var totalIssues = allIssues.Count;
        var openIssues = allIssues.Count(i => i.Status != IssueStatus.Resolved && i.Status != IssueStatus.Closed);
        var criticalIssues = allIssues.Count(i => i.Severity == IssueSeverity.Critical);
        var resolvedIssues = allIssues.Count(i => i.Status == IssueStatus.Resolved || i.Status == IssueStatus.Closed);

        // Material usage
        var totalMaterialCost = visits
            .SelectMany(v => v.MaterialsUsed)
            .Sum(m => m.TotalCost.Amount);

        var materialsUsedCount = visits
            .SelectMany(v => v.MaterialsUsed)
            .Select(m => m.MaterialId)
            .Distinct()
            .Count();

        // Maintenance history
        var maintenanceHistory = visits
            .Where(v => (v.Status == VisitStatus.Completed || v.Status == VisitStatus.Approved) && v.ActualEndTime.HasValue)
            .OrderByDescending(v => v.ActualEndTime)
            .Take(10)
            .Select(v => new SiteMaintenanceHistoryDto
            {
                VisitDate = v.ActualEndTime ?? v.ScheduledDate,
                VisitNumber = v.VisitNumber,
                VisitStatus = v.Status,
                EngineerName = v.EngineerName,
                IssuesFound = v.IssuesFound.Count,
                IssuesResolved = v.IssuesFound.Count(i => i.Status == IssueStatus.Resolved || i.Status == IssueStatus.Closed),
                MaterialCost = v.MaterialsUsed.Sum(m => m.TotalCost.Amount),
                Notes = v.EngineerNotes
            })
            .ToList();

        // Get assigned engineer
        Domain.Entities.Users.User? assignedEngineer = null;
        if (site.AssignedEngineerId.HasValue)
        {
            assignedEngineer = await _userRepository.GetByIdAsNoTrackingAsync(site.AssignedEngineerId.Value, cancellationToken);
        }

        var daysSinceLastVisit = lastVisit != null && lastVisit.ActualEndTime.HasValue
            ? (int)(DateTime.UtcNow - lastVisit.ActualEndTime.Value).TotalDays
            : 0;

        var report = new SiteMaintenanceReportDto
        {
            SiteId = site.Id,
            SiteCode = site.SiteCode.Value,
            SiteName = site.Name,
            Region = site.Region,
            OfficeId = office.Id,
            OfficeName = office.Name,
            Status = site.Status,
            SiteType = site.SiteType,
            Complexity = site.Complexity,
            AssignedEngineerId = site.AssignedEngineerId,
            AssignedEngineerName = assignedEngineer?.Name,
            TotalVisits = totalVisits,
            CompletedVisits = completedVisits,
            LastVisitDate = lastVisit?.ActualEndTime,
            NextScheduledVisit = nextScheduledVisit?.ScheduledDate,
            DaysSinceLastVisit = daysSinceLastVisit,
            TotalIssues = totalIssues,
            OpenIssues = openIssues,
            CriticalIssues = criticalIssues,
            ResolvedIssues = resolvedIssues,
            MaintenanceHistory = maintenanceHistory,
            TotalMaterialCost = totalMaterialCost,
            MaterialsUsedCount = materialsUsedCount,
            FromDate = fromDate,
            ToDate = toDate
        };

        return Result.Success(report);
    }
}

