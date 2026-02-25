using MediatR;
using TowerOps.Application.Common;
using TowerOps.Application.Common.Interfaces;
using TowerOps.Application.DTOs.Sync;
using TowerOps.Application.Services;
using TowerOps.Domain.Entities.Sync;
using TowerOps.Domain.Interfaces.Repositories;

namespace TowerOps.Application.Commands.Sync.ProcessSyncBatch;

public sealed class ProcessSyncBatchCommandHandler : IRequestHandler<ProcessSyncBatchCommand, Result<SyncResultDto>>
{
    private readonly ISyncQueueRepository _syncQueueRepository;
    private readonly ISyncQueueProcessor _syncQueueProcessor;
    private readonly ISystemSettingsService _settingsService;
    private readonly IUnitOfWork _unitOfWork;

    public ProcessSyncBatchCommandHandler(
        ISyncQueueRepository syncQueueRepository,
        ISyncQueueProcessor syncQueueProcessor,
        ISystemSettingsService settingsService,
        IUnitOfWork unitOfWork)
    {
        _syncQueueRepository = syncQueueRepository;
        _syncQueueProcessor = syncQueueProcessor;
        _settingsService = settingsService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<SyncResultDto>> Handle(ProcessSyncBatchCommand request, CancellationToken cancellationToken)
    {
        var maxBatchSize = await _settingsService.GetAsync("Sync:MaxBatchSize", 50, cancellationToken);
        if (request.Items.Count > maxBatchSize)
            return Result.Failure<SyncResultDto>($"Batch size exceeds allowed limit of {maxBatchSize} items.");

        var skipped = 0;
        var queued = new List<SyncQueue>();

        foreach (var item in request.Items.OrderBy(i => i.CreatedOnDeviceUtc))
        {
            var createdOnDeviceUtc = EnsureUtc(item.CreatedOnDeviceUtc);

            var exists = await _syncQueueRepository.ExistsDuplicateAsync(
                request.DeviceId,
                request.EngineerId,
                item.OperationType,
                createdOnDeviceUtc,
                cancellationToken);

            if (exists)
            {
                skipped++;
                continue;
            }

            var queueItem = SyncQueue.Create(
                request.DeviceId,
                request.EngineerId,
                item.OperationType,
                item.Payload,
                createdOnDeviceUtc);

            await _syncQueueRepository.AddAsync(queueItem, cancellationToken);
            queued.Add(queueItem);
        }

        var processResult = await _syncQueueProcessor.ProcessAsync(queued, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new SyncResultDto
        {
            Processed = processResult.Processed,
            Conflicts = processResult.Conflicts,
            Failed = processResult.Failed,
            Skipped = skipped
        });
    }

    private static DateTime EnsureUtc(DateTime value)
    {
        if (value.Kind == DateTimeKind.Utc)
            return value;

        if (value.Kind == DateTimeKind.Unspecified)
            return DateTime.SpecifyKind(value, DateTimeKind.Utc);

        return value.ToUniversalTime();
    }
}
