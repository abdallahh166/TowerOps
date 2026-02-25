using MediatR;
using TowerOps.Application.Commands.Assets;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Assets;
using TowerOps.Domain.Interfaces.Repositories;

namespace TowerOps.Application.Queries.Assets.GetFaultyAssets;

public sealed class GetFaultyAssetsQueryHandler : IRequestHandler<GetFaultyAssetsQuery, Result<IReadOnlyList<AssetDto>>>
{
    private readonly IAssetRepository _assetRepository;

    public GetFaultyAssetsQueryHandler(IAssetRepository assetRepository)
    {
        _assetRepository = assetRepository;
    }

    public async Task<Result<IReadOnlyList<AssetDto>>> Handle(GetFaultyAssetsQuery request, CancellationToken cancellationToken)
    {
        var assets = await _assetRepository.GetFaultyAssetsAsNoTrackingAsync(cancellationToken);
        var result = assets.Select(AssetMapper.ToDto).ToList();
        return Result.Success<IReadOnlyList<AssetDto>>(result);
    }
}
