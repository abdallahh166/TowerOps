namespace TelecomPM.Application.Commands.Users.ActivateUser;

using FluentValidation;
using System;

public class ActivateUserCommandValidator : AbstractValidator<ActivateUserCommand>
{
    public ActivateUserCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");
    }
}

