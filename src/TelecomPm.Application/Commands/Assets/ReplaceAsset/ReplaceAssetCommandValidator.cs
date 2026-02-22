using FluentValidation;

namespace TelecomPM.Application.Commands.Assets.ReplaceAsset;

public sealed class ReplaceAssetCommandValidator : AbstractValidator<ReplaceAssetCommand>
{
    public ReplaceAssetCommandValidator()
    {
        RuleFor(x => x.AssetCode).NotEmpty();
        RuleFor(x => x.NewAssetId).NotEmpty();
    }
}
