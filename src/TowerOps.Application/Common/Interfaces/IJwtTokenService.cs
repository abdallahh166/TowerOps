namespace TowerOps.Application.Common.Interfaces;

using TowerOps.Domain.Entities.Users;

public interface IJwtTokenService
{
    Task<(string token, DateTime expiresAtUtc)> GenerateTokenAsync(User user, CancellationToken cancellationToken = default);
}
