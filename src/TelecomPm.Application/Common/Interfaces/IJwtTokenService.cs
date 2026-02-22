namespace TelecomPM.Application.Common.Interfaces;

using TelecomPM.Domain.Entities.Users;

public interface IJwtTokenService
{
    Task<(string token, DateTime expiresAtUtc)> GenerateTokenAsync(User user, CancellationToken cancellationToken = default);
}
