using FluentValidation;

namespace TelecomPM.Application.Commands.Roles.CreateApplicationRole;

public sealed class CreateApplicationRoleCommandValidator : AbstractValidator<CreateApplicationRoleCommand>
{
    public CreateApplicationRoleCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Role name is required.");

        RuleFor(x => x.DisplayName)
            .NotEmpty()
            .WithMessage("Display name is required.");
    }
}
