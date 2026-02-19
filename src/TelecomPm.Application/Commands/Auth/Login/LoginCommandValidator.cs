namespace TelecomPM.Application.Commands.Auth.Login;

using FluentValidation;

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.PhoneNumber).NotEmpty().MinimumLength(3).MaximumLength(20);
    }
}
