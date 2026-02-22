using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Assets;

namespace TelecomPM.Application.Queries.Assets.GetAssetByCode;

public sealed record GetAssetByCodeQuery : IQuery<AssetDto>
{
    public string AssetCode { get; init; } = string.Empty;
}
