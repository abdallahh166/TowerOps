using MediatR;
using TelecomPM.Application.Commands.Assets;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Assets;
using TelecomPM.Domain.Interfaces.Repositories;

namespace TelecomPM.Application.Queries.Assets.GetFaultyAssets;

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
