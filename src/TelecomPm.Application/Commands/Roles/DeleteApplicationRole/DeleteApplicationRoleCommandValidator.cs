using FluentValidation;

namespace TelecomPM.Application.Commands.Roles.DeleteApplicationRole;

public sealed class DeleteApplicationRoleCommandValidator : AbstractValidator<DeleteApplicationRoleCommand>
{
    public DeleteApplicationRoleCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Role id is required.");
    }
}
