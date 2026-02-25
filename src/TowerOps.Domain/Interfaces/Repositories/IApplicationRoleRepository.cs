using TowerOps.Domain.Entities.ApplicationRoles;

namespace TowerOps.Domain.Interfaces.Repositories;

public interface IApplicationRoleRepository : IRepository<ApplicationRole, string>
{
    Task<ApplicationRole?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ApplicationRole>> GetActiveAsync(CancellationToken cancellationToken = default);
}
