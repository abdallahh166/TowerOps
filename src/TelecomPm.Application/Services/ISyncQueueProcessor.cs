using TelecomPM.Application.DTOs.Sync;
using TelecomPM.Domain.Entities.Sync;

namespace TelecomPM.Application.Services;

public interface ISyncQueueProcessor
{
    Task<SyncResultDto> ProcessAsync(IReadOnlyList<SyncQueue> queuedItems, CancellationToken cancellationToken = default);
}
