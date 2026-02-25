namespace TowerOps.Api.Contracts.Assets;

public sealed class RecordAssetServiceRequest
{
    public string ServiceType { get; init; } = string.Empty;
    public string? EngineerId { get; init; }
    public Guid? VisitId { get; init; }
    public string? Notes { get; init; }
}
