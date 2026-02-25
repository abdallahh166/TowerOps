namespace TowerOps.Application.Commands.Users.UpdateUser;

using FluentValidation;
using System;
using System.Text.RegularExpressions;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("User name is required")
            .MaximumLength(200).WithMessage("User name must not exceed 200 characters");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required")
            .Matches(@"^\+20(10|11|12|15)\d{8}$").WithMessage("Invalid phone number format. Expected format: +20 10/11/12/15 XXXXXXXX");
    }
}

