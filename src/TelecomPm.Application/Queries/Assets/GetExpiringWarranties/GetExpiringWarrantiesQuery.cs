using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Assets;

namespace TelecomPM.Application.Queries.Assets.GetExpiringWarranties;

public sealed record GetExpiringWarrantiesQuery : IQuery<IReadOnlyList<AssetDto>>
{
    public int Days { get; init; } = 30;
}
