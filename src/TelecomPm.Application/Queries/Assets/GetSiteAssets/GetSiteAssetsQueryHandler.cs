using MediatR;
using TelecomPM.Application.Commands.Assets;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Assets;
using TelecomPM.Domain.Interfaces.Repositories;

namespace TelecomPM.Application.Queries.Assets.GetSiteAssets;

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
