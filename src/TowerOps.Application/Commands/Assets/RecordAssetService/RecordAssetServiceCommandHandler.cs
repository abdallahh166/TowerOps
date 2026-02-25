using MediatR;
using TowerOps.Application.Commands.Assets;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Assets;
using TowerOps.Domain.Interfaces.Repositories;

namespace TowerOps.Application.Commands.Assets.RecordAssetService;

public sealed class RecordAssetServiceCommandHandler : IRequestHandler<RecordAssetServiceCommand, Result<AssetDto>>
{
    private readonly IAssetRepository _assetRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RecordAssetServiceCommandHandler(
        IAssetRepository assetRepository,
        IUnitOfWork unitOfWork)
    {
        _assetRepository = assetRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AssetDto>> Handle(RecordAssetServiceCommand request, CancellationToken cancellationToken)
    {
        var asset = await _assetRepository.GetByAssetCodeAsync(request.AssetCode, cancellationToken);
        if (asset is null)
            return Result.Failure<AssetDto>("Asset not found.");

        asset.RecordService(request.ServiceType, request.EngineerId, request.VisitId, request.Notes);

        await _assetRepository.UpdateAsync(asset, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(AssetMapper.ToDto(asset));
    }
}
