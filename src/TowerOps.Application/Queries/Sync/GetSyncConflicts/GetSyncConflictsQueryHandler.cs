using MediatR;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Sync;
using TowerOps.Domain.Interfaces.Repositories;

namespace TowerOps.Application.Queries.Sync.GetSyncConflicts;

public sealed class GetSyncConflictsQueryHandler : IRequestHandler<GetSyncConflictsQuery, Result<IReadOnlyList<SyncConflictDto>>>
{
    private readonly ISyncConflictRepository _syncConflictRepository;

    public GetSyncConflictsQueryHandler(ISyncConflictRepository syncConflictRepository)
    {
        _syncConflictRepository = syncConflictRepository;
    }

    public async Task<Result<IReadOnlyList<SyncConflictDto>>> Handle(GetSyncConflictsQuery request, CancellationToken cancellationToken)
    {
        var conflicts = await _syncConflictRepository.GetByEngineerIdAsNoTrackingAsync(request.EngineerId, cancellationToken);
        var result = conflicts
            .Select(c => new SyncConflictDto
            {
                Id = c.Id,
                SyncQueueId = c.SyncQueueId,
                ConflictType = c.ConflictType,
                Resolution = c.Resolution,
                ResolvedAtUtc = c.ResolvedAtUtc
            })
            .ToList();

        return Result.Success<IReadOnlyList<SyncConflictDto>>(result);
    }
}
