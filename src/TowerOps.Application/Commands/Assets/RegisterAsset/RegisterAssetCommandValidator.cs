using FluentValidation;

namespace TowerOps.Application.Commands.Assets.RegisterAsset;

public sealed class RegisterAssetCommandValidator : AbstractValidator<RegisterAssetCommand>
{
    public RegisterAssetCommandValidator()
    {
        RuleFor(x => x.SiteCode).NotEmpty();
        RuleFor(x => x.InstalledAtUtc).NotEqual(default(DateTime));
    }
}
