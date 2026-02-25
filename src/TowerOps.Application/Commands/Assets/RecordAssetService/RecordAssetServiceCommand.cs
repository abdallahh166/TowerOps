using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Assets;

namespace TowerOps.Application.Commands.Assets.RecordAssetService;

public sealed record RecordAssetServiceCommand : ICommand<AssetDto>
{
    public string AssetCode { get; init; } = string.Empty;
    public string ServiceType { get; init; } = string.Empty;
    public string? EngineerId { get; init; }
    public Guid? VisitId { get; init; }
    public string? Notes { get; init; }
}
