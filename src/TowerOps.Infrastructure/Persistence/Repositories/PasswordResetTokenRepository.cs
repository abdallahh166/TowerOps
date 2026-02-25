namespace TowerOps.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using TowerOps.Domain.Entities.PasswordResetTokens;
using TowerOps.Domain.Interfaces.Repositories;

public sealed class PasswordResetTokenRepository : Repository<PasswordResetToken, Guid>, IPasswordResetTokenRepository
{
    public PasswordResetTokenRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<PasswordResetToken?> GetLatestByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(t => t.Email == email)
            .OrderByDescending(t => t.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
