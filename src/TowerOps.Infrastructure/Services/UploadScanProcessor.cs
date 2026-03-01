using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TowerOps.Application.Common.Interfaces;
using TowerOps.Domain.Enums;
using TowerOps.Domain.Interfaces.Repositories;
using TowerOps.Infrastructure.Persistence;

namespace TowerOps.Infrastructure.Services;

public sealed class UploadScanProcessor
{
    private const int DefaultBatchSize = 100;
    private readonly ApplicationDbContext _context;
    private readonly IFileMalwareScanService _malwareScanService;
    private readonly ISystemSettingsService _settingsService;
    private readonly INotificationService _notificationService;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UploadScanProcessor> _logger;

    public UploadScanProcessor(
        ApplicationDbContext context,
        IFileMalwareScanService malwareScanService,
        ISystemSettingsService settingsService,
        INotificationService notificationService,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        ILogger<UploadScanProcessor> logger)
    {
        _context = context;
        _malwareScanService = malwareScanService;
        _settingsService = settingsService;
        _notificationService = notificationService;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<int> EvaluateBatchAsync(CancellationToken cancellationToken = default)
    {
        var configuredBatchSize = await _settingsService.GetAsync(
            "UploadSecurity:Scan:BatchSize",
            DefaultBatchSize,
            cancellationToken);

        var batchSize = Math.Clamp(configuredBatchSize, 1, 1000);
        var pendingPhotos = await _context.VisitPhotos
            .Include(p => p.Visit)
            .Where(p => p.FileStatus == UploadedFileStatus.Pending)
            .OrderBy(p => p.CreatedAt)
            .Take(batchSize)
            .ToListAsync(cancellationToken);

        if (pendingPhotos.Count == 0)
            return 0;

        foreach (var photo in pendingPhotos)
        {
            var scanResult = await _malwareScanService.ScanAsync(photo.FileName, photo.FilePath, cancellationToken);
            if (scanResult.IsClean)
            {
                photo.MarkApproved("UploadScanner");
                continue;
            }

            var reason = string.IsNullOrWhiteSpace(scanResult.Details)
                ? "Upload quarantined by malware scan policy."
                : scanResult.Details;

            photo.MarkQuarantined(reason, "UploadScanner");
            await NotifyQuarantineAsync(photo, reason, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogDebug(
            "Upload scan cycle processed {Count} pending files.",
            pendingPhotos.Count);

        return pendingPhotos.Count;
    }

    private async Task NotifyQuarantineAsync(
        TowerOps.Domain.Entities.Visits.VisitPhoto photo,
        string reason,
        CancellationToken cancellationToken)
    {
        try
        {
            var visit = photo.Visit;
            if (visit is not null && visit.EngineerId != Guid.Empty)
            {
                await _notificationService.SendPushNotificationAsync(
                    visit.EngineerId,
                    "Upload quarantined",
                    $"A file for visit {visit.VisitNumber} was quarantined. Please re-upload valid evidence.",
                    cancellationToken);

                var engineer = await _userRepository.GetByIdAsNoTrackingAsync(visit.EngineerId, cancellationToken);
                if (engineer is not null && !string.IsNullOrWhiteSpace(engineer.Email))
                {
                    await _notificationService.SendEmailAsync(
                        engineer.Email,
                        "TowerOps Upload Quarantined",
                        $"Your uploaded file '{photo.FileName}' for visit {visit.VisitNumber} was quarantined. Reason: {reason}",
                        cancellationToken);
                }
            }

            var admins = await _userRepository.GetByRoleAsNoTrackingAsync(UserRole.Admin, cancellationToken);
            foreach (var admin in admins)
            {
                if (!string.IsNullOrWhiteSpace(admin.Email))
                {
                    await _notificationService.SendEmailAsync(
                        admin.Email,
                        "TowerOps Upload Security Alert",
                        $"File '{photo.FileName}' was quarantined. Reason: {reason}",
                        cancellationToken);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to send quarantine notifications for photo {PhotoId}", photo.Id);
        }
    }
}
