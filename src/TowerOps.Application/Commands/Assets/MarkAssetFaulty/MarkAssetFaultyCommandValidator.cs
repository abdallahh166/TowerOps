using FluentValidation;

namespace TowerOps.Application.Commands.Assets.MarkAssetFaulty;

public sealed class MarkAssetFaultyCommandValidator : AbstractValidator<MarkAssetFaultyCommand>
{
    public MarkAssetFaultyCommandValidator()
    {
        RuleFor(x => x.AssetCode).NotEmpty();
    }
}
