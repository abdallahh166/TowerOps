using MediatR;
using TelecomPM.Application.Commands.Assets;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Assets;
using TelecomPM.Domain.Interfaces.Repositories;

namespace TelecomPM.Application.Queries.Assets.GetAssetByCode;

public sealed class GetAssetByCodeQueryHandler : IRequestHandler<GetAssetByCodeQuery, Result<AssetDto>>
{
    private readonly IAssetRepository _assetRepository;

    public GetAssetByCodeQueryHandler(IAssetRepository assetRepository)
    {
        _assetRepository = assetRepository;
    }

    public async Task<Result<AssetDto>> Handle(GetAssetByCodeQuery request, CancellationToken cancellationToken)
    {
        var asset = await _assetRepository.GetByAssetCodeAsNoTrackingAsync(request.AssetCode, cancellationToken);
        if (asset is null)
            return Result.Failure<AssetDto>("Asset not found.");

        return Result.Success(AssetMapper.ToDto(asset));
    }
}
