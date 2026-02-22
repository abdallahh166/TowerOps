using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Assets;

namespace TelecomPM.Application.Commands.Assets.RecordAssetService;

public sealed record RecordAssetServiceCommand : ICommand<AssetDto>
{
    public string AssetCode { get; init; } = string.Empty;
    public string ServiceType { get; init; } = string.Empty;
    public string? EngineerId { get; init; }
    public Guid? VisitId { get; init; }
    public string? Notes { get; init; }
}
