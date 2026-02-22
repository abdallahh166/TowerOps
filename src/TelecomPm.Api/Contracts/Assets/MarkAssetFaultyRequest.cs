namespace TelecomPm.Api.Contracts.Assets;

public sealed class MarkAssetFaultyRequest
{
    public string? Reason { get; init; }
    public string? EngineerId { get; init; }
}
