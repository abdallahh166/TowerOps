using MediatR;
using TowerOps.Application.Commands.Assets;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Assets;
using TowerOps.Domain.Entities.Assets;
using TowerOps.Domain.Interfaces.Repositories;

namespace TowerOps.Application.Commands.Assets.RegisterAsset;

public sealed class RegisterAssetCommandHandler : IRequestHandler<RegisterAssetCommand, Result<AssetDto>>
{
    private readonly IAssetRepository _assetRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterAssetCommandHandler(
        IAssetRepository assetRepository,
        IUnitOfWork unitOfWork)
    {
        _assetRepository = assetRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AssetDto>> Handle(RegisterAssetCommand request, CancellationToken cancellationToken)
    {
        var asset = Asset.Create(
            request.SiteCode,
            request.Type,
            request.Brand,
            request.Model,
            request.SerialNumber,
            request.InstalledAtUtc,
            request.WarrantyExpiresAtUtc);

        await _assetRepository.AddAsync(asset, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(AssetMapper.ToDto(asset));
    }
}
