namespace TelecomPM.Application.Commands.Users.ChangeUserRole;

using FluentValidation;
using System;
using TelecomPM.Domain.Enums;

public class ChangeUserRoleCommandValidator : AbstractValidator<ChangeUserRoleCommand>
{
    public ChangeUserRoleCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");

        RuleFor(x => x.NewRole)
            .IsInEnum().WithMessage("Invalid user role");
    }
}

