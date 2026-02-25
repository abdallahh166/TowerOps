namespace TowerOps.Application.DTOs.Signatures;

public sealed record SignatureDto
{
    public string SignerName { get; init; } = string.Empty;
    public string SignerRole { get; init; } = string.Empty;
    public string SignatureDataBase64 { get; init; } = string.Empty;
    public DateTime SignedAtUtc { get; init; }
    public string? SignerPhone { get; init; }
    public decimal? Latitude { get; init; }
    public decimal? Longitude { get; init; }
}

public sealed record WorkOrderSignaturesDto
{
    public SignatureDto? ClientSignature { get; init; }
    public SignatureDto? EngineerSignature { get; init; }
}
