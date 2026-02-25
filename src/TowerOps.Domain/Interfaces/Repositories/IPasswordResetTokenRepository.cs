using TowerOps.Domain.Entities.PasswordResetTokens;

namespace TowerOps.Domain.Interfaces.Repositories;

public interface IPasswordResetTokenRepository : IRepository<PasswordResetToken, Guid>
{
    Task<PasswordResetToken?> GetLatestByEmailAsync(string email, CancellationToken cancellationToken = default);
}
