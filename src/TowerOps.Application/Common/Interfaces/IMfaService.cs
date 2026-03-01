namespace TowerOps.Application.Common.Interfaces;

public interface IMfaService
{
    string GenerateSecret();
    bool VerifyCode(string secret, string code, DateTime nowUtc);
    string BuildOtpAuthUri(string email, string issuer, string secret);
}

