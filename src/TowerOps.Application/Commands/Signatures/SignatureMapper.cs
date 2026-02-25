using TowerOps.Application.DTOs.Signatures;
using TowerOps.Domain.ValueObjects;

namespace TowerOps.Application.Commands.Signatures;

internal static class SignatureMapper
{
    public static SignatureDto ToDto(Signature signature)
    {
        return new SignatureDto
        {
            SignerName = signature.SignerName,
            SignerRole = signature.SignerRole,
            SignatureDataBase64 = signature.SignatureDataBase64,
            SignedAtUtc = signature.SignedAtUtc,
            SignerPhone = signature.SignerPhone,
            Latitude = signature.SignedAtLocation?.Latitude,
            Longitude = signature.SignedAtLocation?.Longitude
        };
    }
}
