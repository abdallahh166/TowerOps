namespace TowerOps.Application.Commands.Users.AssignUserToOffice;

using FluentValidation;
using System;

public class AssignUserToOfficeCommandValidator : AbstractValidator<AssignUserToOfficeCommand>
{
    public AssignUserToOfficeCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");

        RuleFor(x => x.OfficeId)
            .NotEmpty().WithMessage("Office ID is required");
    }
}

