using MediatR;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Sync;
using TowerOps.Domain.Enums;
using TowerOps.Domain.Interfaces.Repositories;

namespace TowerOps.Application.Queries.Sync.GetSyncStatus;

public sealed class GetSyncStatusQueryHandler : IRequestHandler<GetSyncStatusQuery, Result<SyncStatusDto>>
{
    private readonly ISyncQueueRepository _syncQueueRepository;

    public GetSyncStatusQueryHandler(ISyncQueueRepository syncQueueRepository)
    {
        _syncQueueRepository = syncQueueRepository;
    }

    public async Task<Result<SyncStatusDto>> Handle(GetSyncStatusQuery request, CancellationToken cancellationToken)
    {
        var items = await _syncQueueRepository.GetByDeviceIdAsNoTrackingAsync(request.DeviceId, cancellationToken);
        var list = items
            .Select(i => new SyncStatusItemDto
            {
                Id = i.Id,
                OperationType = i.OperationType,
                CreatedOnDeviceUtc = i.CreatedOnDeviceUtc,
                Status = i.Status,
                ConflictReason = i.ConflictReason,
                RetryCount = i.RetryCount
            })
            .ToList();

        return Result.Success(new SyncStatusDto
        {
            DeviceId = request.DeviceId,
            Total = list.Count,
            Pending = list.Count(i => i.Status == SyncStatus.Pending),
            Processed = list.Count(i => i.Status == SyncStatus.Processed),
            Conflicts = list.Count(i => i.Status == SyncStatus.Conflict),
            Failed = list.Count(i => i.Status == SyncStatus.Failed),
            Items = list
        });
    }
}
