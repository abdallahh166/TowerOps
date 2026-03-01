using FluentValidation;

namespace TowerOps.Application.Commands.Auth.GenerateMfaSetup;

public sealed class GenerateMfaSetupCommandValidator : AbstractValidator<GenerateMfaSetupCommand>
{
    public GenerateMfaSetupCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8).MaximumLength(200);
    }
}

