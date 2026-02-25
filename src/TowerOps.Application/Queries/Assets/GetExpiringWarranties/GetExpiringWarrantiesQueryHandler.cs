using MediatR;
using TowerOps.Application.Commands.Assets;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Assets;
using TowerOps.Domain.Interfaces.Repositories;

namespace TowerOps.Application.Queries.Assets.GetExpiringWarranties;

public sealed class GetExpiringWarrantiesQueryHandler : IRequestHandler<GetExpiringWarrantiesQuery, Result<IReadOnlyList<AssetDto>>>
{
    private readonly IAssetRepository _assetRepository;

    public GetExpiringWarrantiesQueryHandler(IAssetRepository assetRepository)
    {
        _assetRepository = assetRepository;
    }

    public async Task<Result<IReadOnlyList<AssetDto>>> Handle(GetExpiringWarrantiesQuery request, CancellationToken cancellationToken)
    {
        var assets = await _assetRepository.GetExpiringWarrantiesAsNoTrackingAsync(request.Days, cancellationToken);
        var result = assets.Select(AssetMapper.ToDto).ToList();
        return Result.Success<IReadOnlyList<AssetDto>>(result);
    }
}
