namespace TowerOps.Api.Contracts.WorkOrders;

public sealed class CaptureWorkOrderSignatureRequest
{
    public string SignerName { get; init; } = string.Empty;
    public string SignerRole { get; init; } = string.Empty;
    public string SignatureDataBase64 { get; init; } = string.Empty;
    public string? SignerPhone { get; init; }
    public decimal? Latitude { get; init; }
    public decimal? Longitude { get; init; }
    public bool IsEngineerSignature { get; init; }
}
