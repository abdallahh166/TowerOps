using System.Security.Cryptography;
using System.Text;
using TowerOps.Application.Common.Interfaces;

namespace TowerOps.Infrastructure.Services;

public sealed class MfaService : IMfaService
{
    private const int TotpDigits = 6;
    private const int TimeStepSeconds = 30;
    private static readonly DateTime UnixEpoch = DateTime.UnixEpoch;

    public string GenerateSecret()
    {
        Span<byte> secretBytes = stackalloc byte[20];
        RandomNumberGenerator.Fill(secretBytes);
        return Base32Encode(secretBytes);
    }

    public bool VerifyCode(string secret, string code, DateTime nowUtc)
    {
        if (string.IsNullOrWhiteSpace(secret) || string.IsNullOrWhiteSpace(code))
            return false;

        if (!int.TryParse(code.Trim(), out _))
            return false;

        var normalizedCode = code.Trim().PadLeft(TotpDigits, '0');
        var secretBytes = Base32Decode(secret);
        var currentCounter = GetTimeCounter(nowUtc);

        // Accept one step skew on both sides to tolerate device clock drift.
        for (var offset = -1L; offset <= 1L; offset++)
        {
            var expected = ComputeTotp(secretBytes, currentCounter + offset);
            if (CryptographicOperations.FixedTimeEquals(
                Encoding.ASCII.GetBytes(expected),
                Encoding.ASCII.GetBytes(normalizedCode)))
            {
                return true;
            }
        }

        return false;
    }

    public string BuildOtpAuthUri(string email, string issuer, string secret)
    {
        var normalizedIssuer = string.IsNullOrWhiteSpace(issuer) ? "TowerOps" : issuer.Trim();
        var normalizedEmail = string.IsNullOrWhiteSpace(email) ? "user@towerops.local" : email.Trim().ToLowerInvariant();
        var label = Uri.EscapeDataString($"{normalizedIssuer}:{normalizedEmail}");
        var issuerParam = Uri.EscapeDataString(normalizedIssuer);
        var secretParam = Uri.EscapeDataString(secret.Trim());
        return $"otpauth://totp/{label}?secret={secretParam}&issuer={issuerParam}&digits={TotpDigits}&period={TimeStepSeconds}";
    }

    private static long GetTimeCounter(DateTime nowUtc)
    {
        var normalized = nowUtc.Kind == DateTimeKind.Utc ? nowUtc : nowUtc.ToUniversalTime();
        var totalSeconds = (long)(normalized - UnixEpoch).TotalSeconds;
        return totalSeconds / TimeStepSeconds;
    }

    private static string ComputeTotp(byte[] secret, long counter)
    {
        Span<byte> counterBytes = stackalloc byte[8];
        var counterValue = BitConverter.GetBytes(counter);
        if (BitConverter.IsLittleEndian)
            Array.Reverse(counterValue);

        counterValue.CopyTo(counterBytes);
        using var hmac = new HMACSHA1(secret);
        var hash = hmac.ComputeHash(counterBytes.ToArray());

        var offset = hash[^1] & 0x0F;
        var binary =
            ((hash[offset] & 0x7F) << 24) |
            ((hash[offset + 1] & 0xFF) << 16) |
            ((hash[offset + 2] & 0xFF) << 8) |
            (hash[offset + 3] & 0xFF);

        var otp = binary % (int)Math.Pow(10, TotpDigits);
        return otp.ToString($"D{TotpDigits}");
    }

    private static string Base32Encode(ReadOnlySpan<byte> data)
    {
        const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
        var output = new StringBuilder((int)Math.Ceiling(data.Length / 5d) * 8);

        int buffer = data[0];
        int next = 1;
        int bitsLeft = 8;

        while (bitsLeft > 0 || next < data.Length)
        {
            if (bitsLeft < 5)
            {
                if (next < data.Length)
                {
                    buffer <<= 8;
                    buffer |= data[next++] & 0xFF;
                    bitsLeft += 8;
                }
                else
                {
                    var pad = 5 - bitsLeft;
                    buffer <<= pad;
                    bitsLeft += pad;
                }
            }

            var index = 0x1F & (buffer >> (bitsLeft - 5));
            bitsLeft -= 5;
            output.Append(alphabet[index]);
        }

        return output.ToString();
    }

    private static byte[] Base32Decode(string base32)
    {
        if (string.IsNullOrWhiteSpace(base32))
            return Array.Empty<byte>();

        var cleaned = base32.Trim().TrimEnd('=').ToUpperInvariant();
        var output = new List<byte>(cleaned.Length * 5 / 8);

        var buffer = 0;
        var bitsLeft = 0;
        foreach (var c in cleaned)
        {
            int val = c switch
            {
                >= 'A' and <= 'Z' => c - 'A',
                >= '2' and <= '7' => c - '2' + 26,
                _ => -1
            };

            if (val < 0)
                throw new FormatException("Invalid Base32 character in MFA secret.");

            buffer <<= 5;
            buffer |= val & 0x1F;
            bitsLeft += 5;

            if (bitsLeft >= 8)
            {
                output.Add((byte)((buffer >> (bitsLeft - 8)) & 0xFF));
                bitsLeft -= 8;
            }
        }

        return output.ToArray();
    }
}

