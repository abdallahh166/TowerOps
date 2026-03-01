namespace TowerOps.Application.Commands.Auth.Login;

using FluentValidation;

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8).MaximumLength(200);

        RuleFor(x => x.MfaCode)
            .Matches(@"^\d{6}$")
            .When(x => !string.IsNullOrWhiteSpace(x.MfaCode))
            .WithMessage("MFA code must be exactly 6 digits.");
    }
}
