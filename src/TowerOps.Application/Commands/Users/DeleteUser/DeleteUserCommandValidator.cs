namespace TowerOps.Application.Commands.Users.DeleteUser;

using FluentValidation;
using System;

public class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
{
    public DeleteUserCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");

        RuleFor(x => x.DeletedBy)
            .NotEmpty().WithMessage("Deleted by is required");
    }
}

