using TelecomPM.Domain.Entities.Users;
using TelecomPM.Domain.Enums;

namespace TelecomPM.Domain.Interfaces.Repositories;

public interface IUserRepository : IRepository<User, Guid>
{
    // ✅ WITH TRACKING - For updates
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<User>> GetByRoleAsync(UserRole role, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<User>> GetByOfficeIdAsync(Guid officeId, CancellationToken cancellationToken = default);

    // ✅ WITHOUT TRACKING - For display/reports
    Task<User?> GetByEmailAsNoTrackingAsync(string email, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<User>> GetByRoleAsNoTrackingAsync(UserRole role, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<User>> GetByOfficeIdAsNoTrackingAsync(Guid officeId, CancellationToken cancellationToken = default);

    // ✅ QUERY METHODS - Always optimized
    Task<bool> IsEmailUniqueAsync(string email, Guid? excludeUserId = null, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
    Task<int> GetUserCountByRoleAsync(UserRole role, CancellationToken cancellationToken = default);
    Task<int> GetActiveUserCountByOfficeAsync(Guid officeId, CancellationToken cancellationToken = default);
}