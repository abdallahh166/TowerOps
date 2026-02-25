using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Assets;

namespace TowerOps.Application.Queries.Assets.GetExpiringWarranties;

public sealed record GetExpiringWarrantiesQuery : IQuery<IReadOnlyList<AssetDto>>
{
    public int Days { get; init; } = 30;
}
