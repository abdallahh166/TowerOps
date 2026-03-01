using TowerOps.Domain.Entities.UserDataExports;

namespace TowerOps.Domain.Interfaces.Repositories;

public interface IUserDataExportRequestRepository : IRepository<UserDataExportRequest, Guid>
{
    Task<UserDataExportRequest?> GetByIdForUserAsync(Guid requestId, Guid userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UserDataExportRequest>> GetPendingOrCompletedExpiringBeforeAsync(DateTime utcNow, int take, CancellationToken cancellationToken = default);
}
