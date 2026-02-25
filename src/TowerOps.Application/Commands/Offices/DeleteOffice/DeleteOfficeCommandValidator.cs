namespace TowerOps.Application.Commands.Offices.DeleteOffice;

using FluentValidation;
using System;

public class DeleteOfficeCommandValidator : AbstractValidator<DeleteOfficeCommand>
{
    public DeleteOfficeCommandValidator()
    {
        RuleFor(x => x.OfficeId)
            .NotEmpty().WithMessage("Office ID is required");

        RuleFor(x => x.DeletedBy)
            .NotEmpty().WithMessage("Deleted by is required");
    }
}

