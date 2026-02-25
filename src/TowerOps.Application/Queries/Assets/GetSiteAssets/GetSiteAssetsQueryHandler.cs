using MediatR;
using TowerOps.Application.Commands.Assets;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Assets;
using TowerOps.Domain.Interfaces.Repositories;

namespace TowerOps.Application.Queries.Assets.GetSiteAssets;

public sealed class GetSiteAssetsQueryHandler : IRequestHandler<GetSiteAssetsQuery, Result<IReadOnlyList<AssetDto>>>
{
    private readonly IAssetRepository _assetRepository;

    public GetSiteAssetsQueryHandler(IAssetRepository assetRepository)
    {
        _assetRepository = assetRepository;
    }

    public async Task<Result<IReadOnlyList<AssetDto>>> Handle(GetSiteAssetsQuery request, CancellationToken cancellationToken)
    {
        var assets = await _assetRepository.GetBySiteCodeAsNoTrackingAsync(request.SiteCode, cancellationToken);
        var result = assets.Select(AssetMapper.ToDto).ToList();
        return Result.Success<IReadOnlyList<AssetDto>>(result);
    }
}
