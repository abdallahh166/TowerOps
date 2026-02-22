using MediatR;
using TelecomPM.Application.Commands.Assets;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Assets;
using TelecomPM.Domain.Interfaces.Repositories;

namespace TelecomPM.Application.Queries.Assets.GetExpiringWarranties;

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
