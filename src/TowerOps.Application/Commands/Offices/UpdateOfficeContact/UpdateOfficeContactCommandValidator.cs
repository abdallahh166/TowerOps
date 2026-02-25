namespace TowerOps.Application.Commands.Offices.UpdateOfficeContact;

using FluentValidation;
using System;

public class UpdateOfficeContactCommandValidator : AbstractValidator<UpdateOfficeContactCommand>
{
    public UpdateOfficeContactCommandValidator()
    {
        RuleFor(x => x.OfficeId)
            .NotEmpty().WithMessage("Office ID is required");

        RuleFor(x => x.ContactPerson)
            .NotEmpty().WithMessage("Contact person is required")
            .MaximumLength(200).WithMessage("Contact person name must not exceed 200 characters");

        RuleFor(x => x.ContactPhone)
            .NotEmpty().WithMessage("Contact phone is required")
            .Matches(@"^\+20(10|11|12|15)\d{8}$").WithMessage("Invalid phone number format. Expected format: +20 10/11/12/15 XXXXXXXX");

        RuleFor(x => x.ContactEmail)
            .NotEmpty().WithMessage("Contact email is required")
            .EmailAddress().WithMessage("Invalid email format");
    }
}

