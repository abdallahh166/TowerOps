using TelecomPM.Application.Common;

namespace TelecomPM.Application.Commands.Signatures.CaptureWorkOrderSignature;

public sealed record CaptureWorkOrderSignatureCommand : ICommand
{
    public Guid WorkOrderId { get; init; }
    public string SignerName { get; init; } = string.Empty;
    public string SignerRole { get; init; } = string.Empty;
    public string SignatureDataBase64 { get; init; } = string.Empty;
    public string? SignerPhone { get; init; }
    public decimal? Latitude { get; init; }
    public decimal? Longitude { get; init; }
    public bool IsEngineerSignature { get; init; }
}
