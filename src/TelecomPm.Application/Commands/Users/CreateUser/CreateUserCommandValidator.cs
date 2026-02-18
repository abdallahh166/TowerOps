namespace TelecomPM.Application.Commands.Users.CreateUser;

using FluentValidation;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using TelecomPM.Domain.Enums;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("User name is required")
            .MaximumLength(200).WithMessage("User name must not exceed 200 characters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(200).WithMessage("Email must not exceed 200 characters");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required")
            .Matches(@"^\+20(10|11|12|15)\d{8}$").WithMessage("Invalid phone number format. Expected format: +20 10/11/12/15 XXXXXXXX");

        RuleFor(x => x.Role)
            .IsInEnum().WithMessage("Invalid user role");

        RuleFor(x => x.OfficeId)
            .NotEmpty().WithMessage("Office ID is required");

        RuleFor(x => x.MaxAssignedSites)
            .GreaterThan(0).WithMessage("Max assigned sites must be greater than zero")
            .When(x => x.MaxAssignedSites.HasValue);

        RuleFor(x => x.MaxAssignedSites)
            .NotNull().WithMessage("Max assigned sites is required for PM Engineers")
            .When(x => x.Role == UserRole.PMEngineer);

        RuleForEach(x => x.Specializations)
            .NotEmpty().WithMessage("Specialization cannot be empty")
            .MaximumLength(100).WithMessage("Specialization must not exceed 100 characters")
            .When(x => x.Specializations != null);
    }
}

