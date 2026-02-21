namespace TelecomPM.Application.Common.Interfaces;

public interface ISettingsEncryptionService
{
    string Encrypt(string plainText);
    string Decrypt(string cipherText);
}
