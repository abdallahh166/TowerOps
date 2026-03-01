namespace TowerOps.Application.Common.Interfaces;

public interface IRefreshTokenService
{
    string GenerateToken();
    string HashToken(string token);
    DateTime GetRefreshTokenExpiryUtc();
}
