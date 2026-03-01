using FluentValidation;

namespace TowerOps.Application.Commands.Auth.VerifyMfaSetup;

public sealed class VerifyMfaSetupCommandValidator : AbstractValidator<VerifyMfaSetupCommand>
{
    public VerifyMfaSetupCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8).MaximumLength(200);
        RuleFor(x => x.Code)
            .NotEmpty()
            .Matches(@"^\d{6}$")
            .WithMessage("MFA code must be exactly 6 digits.");
    }
}
