using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Assets;

namespace TelecomPM.Application.Queries.Assets.GetFaultyAssets;

public sealed record GetFaultyAssetsQuery : IQuery<IReadOnlyList<AssetDto>>;
