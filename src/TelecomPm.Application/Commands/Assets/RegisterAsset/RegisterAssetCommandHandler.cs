using MediatR;
using TelecomPM.Application.Commands.Assets;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Assets;
using TelecomPM.Domain.Entities.Assets;
using TelecomPM.Domain.Interfaces.Repositories;

namespace TelecomPM.Application.Commands.Assets.RegisterAsset;

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
