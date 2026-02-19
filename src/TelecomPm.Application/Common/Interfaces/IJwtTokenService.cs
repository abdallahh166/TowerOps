namespace TelecomPM.Application.Common.Interfaces;

using TelecomPM.Domain.Entities.Users;

public interface IJwtTokenService
{
    (string token, DateTime expiresAtUtc) GenerateToken(User user);
}
