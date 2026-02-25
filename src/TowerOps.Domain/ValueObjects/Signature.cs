using TowerOps.Domain.Exceptions;

namespace TowerOps.Domain.ValueObjects;

public sealed class Signature : ValueObject
{
    private const int MaxBytes = 150 * 1024;
    private static readonly byte[] PngHeader = { 137, 80, 78, 71, 13, 10, 26, 10 };

    public string SignerName { get; private set; } = string.Empty;
    public string SignerRole { get; private set; } = string.Empty;
    public string SignatureDataBase64 { get; private set; } = string.Empty;
    public DateTime SignedAtUtc { get; private set; }
    public string? SignerPhone { get; private set; }
    public GeoLocation? SignedAtLocation { get; private set; }

    private Signature()
    {
    }

    private Signature(
        string signerName,
        string signerRole,
        string signatureDataBase64,
        DateTime signedAtUtc,
        string? signerPhone,
        GeoLocation? signedAtLocation)
    {
        SignerName = signerName;
        SignerRole = signerRole;
        SignatureDataBase64 = signatureDataBase64;
        SignedAtUtc = signedAtUtc;
        SignerPhone = signerPhone;
        SignedAtLocation = signedAtLocation;
    }

    public static Signature Create(
        string signerName,
        string signerRole,
        string signatureDataBase64,
        string? signerPhone = null,
        GeoLocation? signedAtLocation = null)
    {
        if (string.IsNullOrWhiteSpace(signerName))
            throw new DomainException("Signer name is required.", "Signature.SignerName.Required");

        if (string.IsNullOrWhiteSpace(signerRole))
            throw new DomainException("Signer role is required.", "Signature.SignerRole.Required");

        if (string.IsNullOrWhiteSpace(signatureDataBase64))
            throw new DomainException("Signature data is required.", "Signature.Data.Required");

        byte[] bytes;
        try
        {
            bytes = Convert.FromBase64String(signatureDataBase64);
        }
        catch (FormatException)
        {
            throw new DomainException("Signature data must be valid base64.", "Signature.Data.Base64Invalid");
        }

        if (bytes.Length > MaxBytes)
            throw new DomainException("Signature image must be 150KB or less.", "Signature.Data.MaxSize");

        if (bytes.Length < PngHeader.Length || !PngHeader.SequenceEqual(bytes.Take(PngHeader.Length)))
            throw new DomainException("Signature must be a PNG image.", "Signature.Data.PngRequired");

        return new Signature(
            signerName.Trim(),
            signerRole.Trim(),
            signatureDataBase64,
            DateTime.UtcNow,
            signerPhone,
            signedAtLocation);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return SignerName;
        yield return SignerRole;
        yield return SignatureDataBase64;
        yield return SignedAtUtc;
        yield return SignerPhone ?? string.Empty;
        yield return SignedAtLocation?.Latitude ?? decimal.MinValue;
        yield return SignedAtLocation?.Longitude ?? decimal.MinValue;
    }
}
