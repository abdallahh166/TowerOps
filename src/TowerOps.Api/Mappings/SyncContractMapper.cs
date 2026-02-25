using TowerOps.Api.Contracts.Sync;
using TowerOps.Application.Commands.Sync.ProcessSyncBatch;
using TowerOps.Application.Queries.Sync.GetSyncConflicts;
using TowerOps.Application.Queries.Sync.GetSyncStatus;

namespace TowerOps.Api.Mappings;

public static class SyncContractMapper
{
    public static ProcessSyncBatchCommand ToCommand(this SyncBatchRequest request, string engineerId)
        => new()
        {
            DeviceId = request.DeviceId,
            EngineerId = engineerId,
            Items = request.Items
                .Select(i => new SyncBatchItem
                {
                    OperationType = i.OperationType,
                    Payload = i.Payload,
                    CreatedOnDeviceUtc = i.CreatedOnDeviceUtc
                })
                .ToList()
        };

    public static GetSyncStatusQuery ToStatusQuery(this string deviceId)
        => new() { DeviceId = deviceId };

    public static GetSyncConflictsQuery ToConflictsQuery(this string engineerId)
        => new() { EngineerId = engineerId };
}
