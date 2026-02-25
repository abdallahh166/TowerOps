using FluentValidation;

namespace TowerOps.Application.Commands.Assets.RecordAssetService;

public sealed class RecordAssetServiceCommandValidator : AbstractValidator<RecordAssetServiceCommand>
{
    public RecordAssetServiceCommandValidator()
    {
        RuleFor(x => x.AssetCode).NotEmpty();
        RuleFor(x => x.ServiceType).NotEmpty();
    }
}
