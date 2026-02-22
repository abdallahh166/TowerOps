using MediatR;
using TelecomPM.Application.Commands.Assets;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Assets;
using TelecomPM.Domain.Interfaces.Repositories;

namespace TelecomPM.Application.Commands.Assets.ReplaceAsset;

public sealed class ReplaceAssetCommandHandler : IRequestHandler<ReplaceAssetCommand, Result<AssetDto>>
{
    private readonly IAssetRepository _assetRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ReplaceAssetCommandHandler(
        IAssetRepository assetRepository,
        IUnitOfWork unitOfWork)
    {
        _assetRepository = assetRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AssetDto>> Handle(ReplaceAssetCommand request, CancellationToken cancellationToken)
    {
        var asset = await _assetRepository.GetByAssetCodeAsync(request.AssetCode, cancellationToken);
        if (asset is null)
            return Result.Failure<AssetDto>("Asset not found.");

        asset.Replace(request.NewAssetId);
        await _assetRepository.UpdateAsync(asset, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(AssetMapper.ToDto(asset));
    }
}
