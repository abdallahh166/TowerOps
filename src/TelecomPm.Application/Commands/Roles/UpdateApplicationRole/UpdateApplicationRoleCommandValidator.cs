using FluentValidation;

namespace TelecomPM.Application.Commands.Roles.UpdateApplicationRole;

public sealed class UpdateApplicationRoleCommandValidator : AbstractValidator<UpdateApplicationRoleCommand>
{
    public UpdateApplicationRoleCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Role id is required.");

        RuleFor(x => x.DisplayName)
            .NotEmpty()
            .WithMessage("Display name is required.");
    }
}
