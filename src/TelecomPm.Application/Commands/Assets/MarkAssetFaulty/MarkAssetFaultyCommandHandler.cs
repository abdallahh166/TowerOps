using MediatR;
using TelecomPM.Application.Commands.Assets;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Assets;
using TelecomPM.Domain.Interfaces.Repositories;

namespace TelecomPM.Application.Commands.Assets.MarkAssetFaulty;

public sealed class MarkAssetFaultyCommandHandler : IRequestHandler<MarkAssetFaultyCommand, Result<AssetDto>>
{
    private readonly IAssetRepository _assetRepository;
    private readonly IUnitOfWork _unitOfWork;

    public MarkAssetFaultyCommandHandler(
        IAssetRepository assetRepository,
        IUnitOfWork unitOfWork)
    {
        _assetRepository = assetRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AssetDto>> Handle(MarkAssetFaultyCommand request, CancellationToken cancellationToken)
    {
        var asset = await _assetRepository.GetByAssetCodeAsync(request.AssetCode, cancellationToken);
        if (asset is null)
            return Result.Failure<AssetDto>("Asset not found.");

        asset.MarkFaulty(request.Reason, request.EngineerId);

        await _assetRepository.UpdateAsync(asset, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(AssetMapper.ToDto(asset));
    }
}
