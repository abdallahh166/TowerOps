namespace TelecomPM.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using TelecomPM.Application.Common.Interfaces;
using TelecomPM.Application.DTOs.Portal;
using TelecomPM.Domain.Enums;

public sealed class PortalReadRepository : IPortalReadRepository
{
    private readonly ApplicationDbContext _context;

    public PortalReadRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PortalDashboardDto> GetDashboardAsync(string clientCode, CancellationToken cancellationToken = default)
    {
        var normalizedClientCode = NormalizeClientCode(clientCode);
        var nowUtc = DateTime.UtcNow;

        var clientSitesQuery = _context.Sites
            .AsNoTracking()
            .Where(s => s.ClientCode == normalizedClientCode);

        var totalSites = await clientSitesQuery.CountAsync(cancellationToken);
        var onAirSites = await clientSitesQuery.CountAsync(s => s.Status == SiteStatus.OnAir, cancellationToken);

        var clientSiteCodesQuery = clientSitesQuery.Select(s => s.SiteCode.Value);
        var clientWorkOrdersQuery = _context.WorkOrders
            .AsNoTracking()
            .Where(w => clientSiteCodesQuery.Contains(w.SiteCode));

        var closedCount = await clientWorkOrdersQuery
            .CountAsync(w => w.Status == WorkOrderStatus.Closed, cancellationToken);

        var compliantClosedCount = await clientWorkOrdersQuery
            .CountAsync(w =>
                w.Status == WorkOrderStatus.Closed &&
                (w.SlaClass == SlaClass.P4 || nowUtc <= w.ResolutionDeadlineUtc), cancellationToken);

        var pendingCmCount = await clientWorkOrdersQuery
            .CountAsync(w =>
                w.Status != WorkOrderStatus.Closed &&
                w.Status != WorkOrderStatus.Cancelled &&
                (w.SlaClass == SlaClass.P1 || w.SlaClass == SlaClass.P2 || w.SlaClass == SlaClass.P3), cancellationToken);

        var overdueBmCount = await clientWorkOrdersQuery
            .CountAsync(w =>
                w.SlaClass == SlaClass.P4 &&
                w.Status != WorkOrderStatus.Closed &&
                w.Status != WorkOrderStatus.Cancelled &&
                nowUtc > w.ResolutionDeadlineUtc, cancellationToken);

        var onAirPercent = totalSites == 0
            ? 0m
            : decimal.Round(onAirSites * 100m / totalSites, 2);

        var slaCompliance = closedCount == 0
            ? 0m
            : decimal.Round(compliantClosedCount * 100m / closedCount, 2);

        return new PortalDashboardDto
        {
            TotalSites = totalSites,
            OnAirPercent = onAirPercent,
            SlaComplianceRatePercent = slaCompliance,
            PendingCmCount = pendingCmCount,
            OverdueBmCount = overdueBmCount
        };
    }

    public async Task<IReadOnlyList<PortalSiteDto>> GetSitesAsync(
        string clientCode,
        string? siteCode,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var normalizedClientCode = NormalizeClientCode(clientCode);
        var nowUtc = DateTime.UtcNow;
        var safePageNumber = pageNumber < 1 ? 1 : pageNumber;
        var safePageSize = Math.Clamp(pageSize, 1, 200);
        var skip = (safePageNumber - 1) * safePageSize;

        var sitesQuery = _context.Sites
            .AsNoTracking()
            .Where(s => s.ClientCode == normalizedClientCode);

        if (!string.IsNullOrWhiteSpace(siteCode))
        {
            sitesQuery = sitesQuery.Where(s => s.SiteCode.Value == siteCode.Trim());
        }

        var pagedSites = await sitesQuery
            .OrderBy(s => s.SiteCode.Value)
            .Select(s => new
            {
                s.Id,
                SiteCode = s.SiteCode.Value,
                s.Name,
                s.Status,
                s.Region
            })
            .Skip(skip)
            .Take(safePageSize)
            .ToListAsync(cancellationToken);

        if (pagedSites.Count == 0)
            return Array.Empty<PortalSiteDto>();

        var siteIds = pagedSites.Select(s => s.Id).ToList();
        var siteCodes = pagedSites.Select(s => s.SiteCode).ToList();

        var lastVisitBySite = await _context.Visits
            .AsNoTracking()
            .Where(v => siteIds.Contains(v.SiteId))
            .GroupBy(v => v.SiteId)
            .Select(g => new
            {
                SiteId = g.Key,
                LastVisitDate = g.Max(v => (DateTime?)v.ScheduledDate),
                LastVisitType = g
                    .OrderByDescending(v => v.ScheduledDate)
                    .Select(v => (VisitType?)v.Type)
                    .FirstOrDefault()
            })
            .ToListAsync(cancellationToken);

        var workOrderStatsBySiteCode = await _context.WorkOrders
            .AsNoTracking()
            .Where(w => siteCodes.Contains(w.SiteCode))
            .GroupBy(w => w.SiteCode)
            .Select(g => new
            {
                SiteCode = g.Key,
                OpenWorkOrdersCount = g.Count(w => w.Status != WorkOrderStatus.Closed && w.Status != WorkOrderStatus.Cancelled),
                BreachedSlaCount = g.Count(w =>
                    w.Status != WorkOrderStatus.Closed &&
                    w.Status != WorkOrderStatus.Cancelled &&
                    nowUtc > w.ResolutionDeadlineUtc)
            })
            .ToListAsync(cancellationToken);

        var latestVisitsLookup = lastVisitBySite.ToDictionary(x => x.SiteId);
        var workOrderStatsLookup = workOrderStatsBySiteCode.ToDictionary(x => x.SiteCode, StringComparer.OrdinalIgnoreCase);

        return pagedSites
            .Select(s =>
            {
                latestVisitsLookup.TryGetValue(s.Id, out var lastVisit);
                workOrderStatsLookup.TryGetValue(s.SiteCode, out var workOrderStats);

                return new PortalSiteDto
                {
                    SiteCode = s.SiteCode,
                    Name = s.Name,
                    Status = s.Status,
                    Region = s.Region,
                    LastVisitDate = lastVisit?.LastVisitDate,
                    LastVisitType = lastVisit?.LastVisitType,
                    OpenWorkOrdersCount = workOrderStats?.OpenWorkOrdersCount ?? 0,
                    BreachedSlaCount = workOrderStats?.BreachedSlaCount ?? 0
                };
            })
            .ToList();
    }

    public async Task<IReadOnlyList<PortalWorkOrderDto>> GetWorkOrdersAsync(
        string clientCode,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var normalizedClientCode = NormalizeClientCode(clientCode);
        var safePageNumber = pageNumber < 1 ? 1 : pageNumber;
        var safePageSize = Math.Clamp(pageSize, 1, 200);
        var skip = (safePageNumber - 1) * safePageSize;

        var query = from workOrder in _context.WorkOrders.AsNoTracking()
                    join site in _context.Sites.AsNoTracking()
                        on workOrder.SiteCode equals site.SiteCode.Value
                    where site.ClientCode == normalizedClientCode
                    orderby workOrder.CreatedAt descending
                    select new PortalWorkOrderDto
                    {
                        WorkOrderId = workOrder.Id,
                        SiteCode = workOrder.SiteCode,
                        Status = workOrder.Status,
                        Priority = workOrder.SlaClass,
                        SlaDeadline = workOrder.ResolutionDeadlineUtc,
                        CreatedAt = workOrder.CreatedAt,
                        CompletedAt = workOrder.Status == WorkOrderStatus.Closed ? workOrder.UpdatedAt : null
                    };

        return await query
            .Skip(skip)
            .Take(safePageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<PortalVisitDto>> GetVisitsAsync(
        string clientCode,
        string siteCode,
        int pageNumber,
        int pageSize,
        bool anonymizeEngineers,
        CancellationToken cancellationToken = default)
    {
        var normalizedClientCode = NormalizeClientCode(clientCode);
        var normalizedSiteCode = string.IsNullOrWhiteSpace(siteCode) ? string.Empty : siteCode.Trim();
        var safePageNumber = pageNumber < 1 ? 1 : pageNumber;
        var safePageSize = Math.Clamp(pageSize, 1, 200);
        var skip = (safePageNumber - 1) * safePageSize;
        var engineerName = anonymizeEngineers ? "Field Engineer" : null;

        var query =
            from visit in _context.Visits.AsNoTracking()
            join site in _context.Sites.AsNoTracking() on visit.SiteId equals site.Id
            where site.ClientCode == normalizedClientCode &&
                  site.SiteCode.Value == normalizedSiteCode
            orderby visit.ScheduledDate descending
            select new PortalVisitDto
            {
                VisitId = visit.Id,
                VisitNumber = visit.VisitNumber,
                Status = visit.Status,
                Type = visit.Type,
                ScheduledDate = visit.ScheduledDate,
                EngineerDisplayName = engineerName ?? visit.EngineerName
            };

        return await query
            .Skip(skip)
            .Take(safePageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<PortalVisitEvidenceDto?> GetVisitEvidenceAsync(
        string clientCode,
        Guid visitId,
        CancellationToken cancellationToken = default)
    {
        var normalizedClientCode = NormalizeClientCode(clientCode);

        var visitProjection = await (
            from visit in _context.Visits.AsNoTracking()
            join site in _context.Sites.AsNoTracking() on visit.SiteId equals site.Id
            where visit.Id == visitId && site.ClientCode == normalizedClientCode
            select new
            {
                visit.Id,
                visit.VisitNumber,
                SiteCode = site.SiteCode.Value,
                visit.Type,
                visit.Status,
                visit.ScheduledDate
            }).FirstOrDefaultAsync(cancellationToken);

        if (visitProjection is null)
            return null;

        var photos = await _context.VisitPhotos
            .AsNoTracking()
            .Where(p => p.VisitId == visitId)
            .OrderBy(p => p.CapturedAtUtc ?? p.CreatedAt)
            .Select(p => new PortalVisitPhotoEvidenceDto
            {
                PhotoId = p.Id,
                Type = p.Type,
                Category = p.Category,
                ItemName = p.ItemName,
                FileName = p.FileName,
                CapturedAtUtc = p.CapturedAtUtc
            })
            .ToListAsync(cancellationToken);

        var readings = await _context.VisitReadings
            .AsNoTracking()
            .Where(r => r.VisitId == visitId)
            .OrderBy(r => r.MeasuredAt)
            .Select(r => new PortalVisitReadingEvidenceDto
            {
                ReadingId = r.Id,
                ReadingType = r.ReadingType,
                Category = r.Category,
                Value = r.Value,
                Unit = r.Unit,
                IsWithinRange = r.IsWithinRange,
                MeasuredAtUtc = r.MeasuredAt
            })
            .ToListAsync(cancellationToken);

        var checklistItems = await _context.VisitChecklists
            .AsNoTracking()
            .Where(c => c.VisitId == visitId)
            .OrderBy(c => c.Category)
            .ThenBy(c => c.ItemName)
            .Select(c => new PortalVisitChecklistEvidenceDto
            {
                ChecklistItemId = c.Id,
                Category = c.Category,
                ItemName = c.ItemName,
                Status = c.Status,
                IsMandatory = c.IsMandatory,
                CompletedAtUtc = c.CompletedAt
            })
            .ToListAsync(cancellationToken);

        return new PortalVisitEvidenceDto
        {
            VisitId = visitProjection.Id,
            VisitNumber = visitProjection.VisitNumber,
            SiteCode = visitProjection.SiteCode,
            VisitType = visitProjection.Type,
            VisitStatus = visitProjection.Status,
            ScheduledDateUtc = visitProjection.ScheduledDate,
            Photos = photos,
            Readings = readings,
            ChecklistItems = checklistItems
        };
    }

    public async Task<PortalSlaReportDto> GetSlaReportAsync(string clientCode, CancellationToken cancellationToken = default)
    {
        var normalizedClientCode = NormalizeClientCode(clientCode);
        var nowUtc = DateTime.UtcNow;

        var clientSiteCodesQuery = _context.Sites
            .AsNoTracking()
            .Where(s => s.ClientCode == normalizedClientCode)
            .Select(s => s.SiteCode.Value);

        var aggregated = await _context.WorkOrders
            .AsNoTracking()
            .Where(w => clientSiteCodesQuery.Contains(w.SiteCode))
            .GroupBy(w => new { w.CreatedAt.Year, w.CreatedAt.Month, w.SlaClass })
            .Select(g => new
            {
                g.Key.Year,
                g.Key.Month,
                g.Key.SlaClass,
                Total = g.Count(),
                Breaches = g.Count(x =>
                    (x.Status == WorkOrderStatus.Closed
                        ? x.UpdatedAt
                        : nowUtc) > x.ResolutionDeadlineUtc),
                AverageResponseMinutes = g.Average(x =>
                    x.AssignedAtUtc.HasValue
                        ? (double?)EF.Functions.DateDiffMinute(x.CreatedAt, x.AssignedAtUtc.Value)
                        : null)
            })
            .ToListAsync(cancellationToken);

        var monthly = aggregated
            .Select(x =>
            {
                var compliant = x.Total - x.Breaches;
                var compliancePercent = x.Total == 0 ? 0m : decimal.Round(compliant * 100m / x.Total, 2);
                var averageResponseMinutes = x.AverageResponseMinutes.HasValue
                    ? decimal.Round((decimal)x.AverageResponseMinutes.Value, 2)
                    : 0m;

                return new PortalSlaMonthlyMetricDto
                {
                    Year = x.Year,
                    Month = x.Month,
                    SlaClass = x.SlaClass,
                    CompliancePercent = compliancePercent,
                    BreachCount = x.Breaches,
                    AverageResponseMinutes = averageResponseMinutes
                };
            })
            .OrderBy(x => x.Year)
            .ThenBy(x => x.Month)
            .ThenBy(x => x.SlaClass)
            .ToList();

        return new PortalSlaReportDto
        {
            Monthly = monthly
        };
    }

    private static string NormalizeClientCode(string clientCode)
    {
        return clientCode.Trim().ToUpperInvariant();
    }
}
