using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Assets;

namespace TowerOps.Application.Queries.Assets.GetFaultyAssets;

public sealed record GetFaultyAssetsQuery : IQuery<IReadOnlyList<AssetDto>>;
