using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using TelecomPM.Application.Common.Interfaces;

namespace TelecomPM.Infrastructure.Services;

public sealed class SettingsEncryptionService : ISettingsEncryptionService
{
    private readonly byte[] _key;

    public SettingsEncryptionService(IConfiguration configuration)
    {
        var configuredKey = configuration["Settings:EncryptionKey"];
        if (string.IsNullOrWhiteSpace(configuredKey))
        {
            throw new InvalidOperationException("Settings:EncryptionKey is required.");
        }

        // Normalize any input to a deterministic 256-bit key.
        _key = SHA256.HashData(Encoding.UTF8.GetBytes(configuredKey.Trim()));
    }

    public string Encrypt(string plainText)
    {
        if (plainText is null)
            return string.Empty;

        using var aes = Aes.Create();
        aes.Key = _key;
        aes.GenerateIV();

        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        var plainBytes = Encoding.UTF8.GetBytes(plainText);
        var cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

        var payload = new byte[aes.IV.Length + cipherBytes.Length];
        Buffer.BlockCopy(aes.IV, 0, payload, 0, aes.IV.Length);
        Buffer.BlockCopy(cipherBytes, 0, payload, aes.IV.Length, cipherBytes.Length);

        return Convert.ToBase64String(payload);
    }

    public string Decrypt(string cipherText)
    {
        if (string.IsNullOrWhiteSpace(cipherText))
            return string.Empty;

        var payload = Convert.FromBase64String(cipherText);

        using var aes = Aes.Create();
        aes.Key = _key;

        var ivLength = aes.BlockSize / 8;
        if (payload.Length <= ivLength)
            throw new InvalidOperationException("Encrypted payload is invalid.");

        var iv = new byte[ivLength];
        var cipherBytes = new byte[payload.Length - ivLength];
        Buffer.BlockCopy(payload, 0, iv, 0, ivLength);
        Buffer.BlockCopy(payload, ivLength, cipherBytes, 0, cipherBytes.Length);

        using var decryptor = aes.CreateDecryptor(aes.Key, iv);
        var plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
        return Encoding.UTF8.GetString(plainBytes);
    }
}
