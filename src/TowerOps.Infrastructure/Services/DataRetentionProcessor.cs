using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TowerOps.Application.Common.Interfaces;
using TowerOps.Domain.Interfaces.Repositories;
using TowerOps.Infrastructure.Persistence;

namespace TowerOps.Infrastructure.Services;

public sealed class DataRetentionProcessor
{
    private readonly ApplicationDbContext _context;
    private readonly ISystemSettingsService _settingsService;
    private readonly IUserDataExportRequestRepository _userDataExportRequestRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DataRetentionProcessor> _logger;

    public DataRetentionProcessor(
        ApplicationDbContext context,
        ISystemSettingsService settingsService,
        IUserDataExportRequestRepository userDataExportRequestRepository,
        IUnitOfWork unitOfWork,
        ILogger<DataRetentionProcessor> logger)
    {
        _context = context;
        _settingsService = settingsService;
        _userDataExportRequestRepository = userDataExportRequestRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<RetentionCleanupResult> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var cleanupBatchSize = await _settingsService.GetAsync("Privacy:Retention:CleanupBatchSize", 200, cancellationToken);
        var softDeleteGraceDays = await _settingsService.GetAsync("Privacy:Retention:SoftDeleteGraceDays", 90, cancellationToken);
        var operationalRetentionYears = await _settingsService.GetAsync("Privacy:Retention:OperationalYears", 5, cancellationToken);
        var signatureRetentionYears = await _settingsService.GetAsync("Privacy:Retention:SignatureYears", 7, cancellationToken);
        var auditRetentionYears = await _settingsService.GetAsync("Privacy:Retention:AuditLogYears", 7, cancellationToken);

        var safeBatchSize = Math.Clamp(cleanupBatchSize, 10, 5000);
        var safeGraceDays = Math.Clamp(softDeleteGraceDays, 30, 3650);
        var effectiveOperationalYears = Math.Clamp(Math.Max(operationalRetentionYears, signatureRetentionYears), 1, 20);
        var safeAuditYears = Math.Clamp(auditRetentionYears, 1, 20);

        var hardDeleteCutoff = now.AddDays(-safeGraceDays);
        var operationalCutoff = now.AddYears(-effectiveOperationalYears);
        var auditCutoff = now.AddYears(-safeAuditYears);

        var result = new RetentionCleanupResult();

        result.HardDeletedUsers = await HardDeleteSoftDeletedUsersAsync(hardDeleteCutoff, safeBatchSize, cancellationToken);
        result.HardDeletedPhotos = await HardDeleteSoftDeletedPhotosAsync(hardDeleteCutoff, safeBatchSize, cancellationToken);
        result.ExpiredExports = await ExpireDataExportRequestsAsync(now, safeBatchSize, cancellationToken);
        result.HardDeletedOldVisits = await HardDeleteOldVisitsAsync(operationalCutoff, safeBatchSize, cancellationToken);
        result.HardDeletedOldWorkOrders = await HardDeleteOldWorkOrdersAsync(operationalCutoff, safeBatchSize, cancellationToken);
        result.HardDeletedOldAuditLogs = await HardDeleteOldAuditLogsAsync(auditCutoff, safeBatchSize, cancellationToken);

        if (result.TotalAffectedRows > 0)
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        _logger.LogInformation(
            "Data retention cycle completed. Users={Users}, Photos={Photos}, ExportsExpired={Exports}, Visits={Visits}, WorkOrders={WorkOrders}, AuditLogs={AuditLogs}",
            result.HardDeletedUsers,
            result.HardDeletedPhotos,
            result.ExpiredExports,
            result.HardDeletedOldVisits,
            result.HardDeletedOldWorkOrders,
            result.HardDeletedOldAuditLogs);

        return result;
    }

    private async Task<int> HardDeleteSoftDeletedUsersAsync(DateTime cutoffUtc, int take, CancellationToken cancellationToken)
    {
        var rows = await _context.Users
            .IgnoreQueryFilters()
            .Where(u =>
                u.IsDeleted &&
                !u.IsUnderLegalHold &&
                u.DeletedAt.HasValue &&
                u.DeletedAt.Value <= cutoffUtc)
            .OrderBy(u => u.DeletedAt)
            .Take(take)
            .ToListAsync(cancellationToken);

        if (rows.Count > 0)
            _context.Users.RemoveRange(rows);

        return rows.Count;
    }

    private async Task<int> HardDeleteSoftDeletedPhotosAsync(DateTime cutoffUtc, int take, CancellationToken cancellationToken)
    {
        var rows = await _context.VisitPhotos
            .IgnoreQueryFilters()
            .Where(p =>
                p.IsDeleted &&
                !p.IsUnderLegalHold &&
                p.DeletedAt.HasValue &&
                p.DeletedAt.Value <= cutoffUtc)
            .OrderBy(p => p.DeletedAt)
            .Take(take)
            .ToListAsync(cancellationToken);

        if (rows.Count > 0)
            _context.VisitPhotos.RemoveRange(rows);

        return rows.Count;
    }

    private async Task<int> ExpireDataExportRequestsAsync(DateTime nowUtc, int take, CancellationToken cancellationToken)
    {
        var rows = await _userDataExportRequestRepository.GetPendingOrCompletedExpiringBeforeAsync(nowUtc, take, cancellationToken);
        foreach (var item in rows)
        {
            item.MarkExpired(nowUtc);
            await _userDataExportRequestRepository.UpdateAsync(item, cancellationToken);
        }

        return rows.Count;
    }

    private async Task<int> HardDeleteOldVisitsAsync(DateTime cutoffUtc, int take, CancellationToken cancellationToken)
    {
        var rows = await _context.Visits
            .IgnoreQueryFilters()
            .Where(v =>
                !v.IsUnderLegalHold &&
                v.CreatedAt <= cutoffUtc)
            .OrderBy(v => v.CreatedAt)
            .Take(take)
            .ToListAsync(cancellationToken);

        if (rows.Count > 0)
            _context.Visits.RemoveRange(rows);

        return rows.Count;
    }

    private async Task<int> HardDeleteOldWorkOrdersAsync(DateTime cutoffUtc, int take, CancellationToken cancellationToken)
    {
        var rows = await _context.WorkOrders
            .IgnoreQueryFilters()
            .Where(w =>
                !w.IsUnderLegalHold &&
                w.CreatedAt <= cutoffUtc)
            .OrderBy(w => w.CreatedAt)
            .Take(take)
            .ToListAsync(cancellationToken);

        if (rows.Count > 0)
            _context.WorkOrders.RemoveRange(rows);

        return rows.Count;
    }

    private async Task<int> HardDeleteOldAuditLogsAsync(DateTime cutoffUtc, int take, CancellationToken cancellationToken)
    {
        var rows = await _context.AuditLogs
            .IgnoreQueryFilters()
            .Where(a =>
                !a.IsUnderLegalHold &&
                a.OccurredAtUtc <= cutoffUtc)
            .OrderBy(a => a.OccurredAtUtc)
            .Take(take)
            .ToListAsync(cancellationToken);

        if (rows.Count > 0)
            _context.AuditLogs.RemoveRange(rows);

        return rows.Count;
    }
}

public sealed class RetentionCleanupResult
{
    public int HardDeletedUsers { get; set; }
    public int HardDeletedPhotos { get; set; }
    public int ExpiredExports { get; set; }
    public int HardDeletedOldVisits { get; set; }
    public int HardDeletedOldWorkOrders { get; set; }
    public int HardDeletedOldAuditLogs { get; set; }
    public int TotalAffectedRows =>
        HardDeletedUsers +
        HardDeletedPhotos +
        ExpiredExports +
        HardDeletedOldVisits +
        HardDeletedOldWorkOrders +
        HardDeletedOldAuditLogs;
}
