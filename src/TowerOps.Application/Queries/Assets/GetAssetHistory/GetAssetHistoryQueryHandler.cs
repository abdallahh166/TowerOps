using MediatR;
using TowerOps.Application.Commands.Assets;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Assets;
using TowerOps.Domain.Interfaces.Repositories;

namespace TowerOps.Application.Queries.Assets.GetAssetHistory;

public sealed class GetAssetHistoryQueryHandler : IRequestHandler<GetAssetHistoryQuery, Result<AssetDto>>
{
    private readonly IAssetRepository _assetRepository;

    public GetAssetHistoryQueryHandler(IAssetRepository assetRepository)
    {
        _assetRepository = assetRepository;
    }

    public async Task<Result<AssetDto>> Handle(GetAssetHistoryQuery request, CancellationToken cancellationToken)
    {
        var asset = await _assetRepository.GetByAssetCodeAsNoTrackingAsync(request.AssetCode, cancellationToken);
        if (asset is null)
            return Result.Failure<AssetDto>("Asset not found.");

        return Result.Success(AssetMapper.ToDto(asset));
    }
}
