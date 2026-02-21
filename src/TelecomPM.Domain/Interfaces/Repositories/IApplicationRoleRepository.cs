using TelecomPM.Domain.Entities.ApplicationRoles;

namespace TelecomPM.Domain.Interfaces.Repositories;

public interface IApplicationRoleRepository : IRepository<ApplicationRole, string>
{
    Task<ApplicationRole?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ApplicationRole>> GetActiveAsync(CancellationToken cancellationToken = default);
}
