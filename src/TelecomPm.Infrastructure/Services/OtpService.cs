using System.Security.Cryptography;
using System.Text;
using TelecomPM.Application.Common.Interfaces;

namespace TelecomPM.Infrastructure.Services;

public sealed class OtpService : IOtpService
{
    public string GenerateOtp()
    {
        var value = RandomNumberGenerator.GetInt32(0, 1_000_000);
        return value.ToString("D6");
    }

    public string HashOtp(string otp)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(otp ?? string.Empty));
        return Convert.ToBase64String(bytes);
    }

    public bool VerifyOtp(string otp, string hashedOtp)
    {
        if (string.IsNullOrWhiteSpace(otp) || string.IsNullOrWhiteSpace(hashedOtp))
            return false;

        var expected = Convert.FromBase64String(hashedOtp);
        var actual = SHA256.HashData(Encoding.UTF8.GetBytes(otp));
        return CryptographicOperations.FixedTimeEquals(expected, actual);
    }
}
