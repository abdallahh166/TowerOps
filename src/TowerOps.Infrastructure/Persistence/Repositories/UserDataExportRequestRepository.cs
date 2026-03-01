using Microsoft.EntityFrameworkCore;
using TowerOps.Domain.Entities.UserDataExports;
using TowerOps.Domain.Enums;
using TowerOps.Domain.Interfaces.Repositories;

namespace TowerOps.Infrastructure.Persistence.Repositories;

public sealed class UserDataExportRequestRepository : Repository<UserDataExportRequest, Guid>, IUserDataExportRequestRepository
{
    public UserDataExportRequestRepository(ApplicationDbContext context) : base(context)
    {
    }

    public Task<UserDataExportRequest?> GetByIdForUserAsync(Guid requestId, Guid userId, CancellationToken cancellationToken = default)
    {
        return _context.UserDataExportRequests
            .FirstOrDefaultAsync(x => x.Id == requestId && x.UserId == userId, cancellationToken);
    }

    public async Task<IReadOnlyList<UserDataExportRequest>> GetPendingOrCompletedExpiringBeforeAsync(
        DateTime utcNow,
        int take,
        CancellationToken cancellationToken = default)
    {
        var maxTake = Math.Clamp(take, 1, 1000);
        return await _context.UserDataExportRequests
            .Where(x =>
                (x.Status == UserDataExportStatus.Pending ||
                 x.Status == UserDataExportStatus.Completed) &&
                x.ExpiresAtUtc <= utcNow)
            .OrderBy(x => x.ExpiresAtUtc)
            .Take(maxTake)
            .ToListAsync(cancellationToken);
    }
}
