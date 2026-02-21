namespace TelecomPM.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using TelecomPM.Domain.Entities.ApplicationRoles;
using TelecomPM.Domain.Interfaces.Repositories;

public sealed class ApplicationRoleRepository : Repository<ApplicationRole, string>, IApplicationRoleRepository
{
    public ApplicationRoleRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<ApplicationRole?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(r => r.Name == name, cancellationToken);
    }

    public async Task<IReadOnlyList<ApplicationRole>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(r => r.IsActive)
            .OrderBy(r => r.Name)
            .ToListAsync(cancellationToken);
    }
}
