using TowerOps.Application.Common;

namespace TowerOps.Application.Commands.Signatures.CaptureVisitSignature;

public sealed record CaptureVisitSignatureCommand : ICommand
{
    public Guid VisitId { get; init; }
    public string SignerName { get; init; } = string.Empty;
    public string SignerRole { get; init; } = string.Empty;
    public string SignatureDataBase64 { get; init; } = string.Empty;
    public string? SignerPhone { get; init; }
    public decimal? Latitude { get; init; }
    public decimal? Longitude { get; init; }
}
