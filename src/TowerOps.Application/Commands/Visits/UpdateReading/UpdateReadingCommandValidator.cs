namespace TowerOps.Application.Commands.Visits.UpdateReading;

using FluentValidation;
using System;

public class UpdateReadingCommandValidator : AbstractValidator<UpdateReadingCommand>
{
    public UpdateReadingCommandValidator()
    {
        RuleFor(x => x.VisitId)
            .NotEmpty().WithMessage("Visit ID is required");

        RuleFor(x => x.ReadingId)
            .NotEmpty().WithMessage("Reading ID is required");

        RuleFor(x => x.Value)
            .NotEmpty().WithMessage("Reading value is required");
    }
}

