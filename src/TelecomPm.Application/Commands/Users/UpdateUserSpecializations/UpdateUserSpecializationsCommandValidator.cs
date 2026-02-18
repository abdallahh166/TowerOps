namespace TelecomPM.Application.Commands.Users.UpdateUserSpecializations;

using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;

public class UpdateUserSpecializationsCommandValidator : AbstractValidator<UpdateUserSpecializationsCommand>
{
    public UpdateUserSpecializationsCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");

        RuleFor(x => x.MaxAssignedSites)
            .GreaterThan(0).WithMessage("Max assigned sites must be greater than zero");

        RuleForEach(x => x.Specializations)
            .NotEmpty().WithMessage("Specialization cannot be empty")
            .MaximumLength(100).WithMessage("Specialization must not exceed 100 characters");
    }
}

