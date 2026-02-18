namespace TelecomPM.Application.Commands.Visits.RescheduleVisit;

using FluentValidation;
using System;

public class RescheduleVisitCommandValidator : AbstractValidator<RescheduleVisitCommand>
{
    public RescheduleVisitCommandValidator()
    {
        RuleFor(x => x.VisitId)
            .NotEmpty().WithMessage("Visit ID is required");

        RuleFor(x => x.NewScheduledDate)
            .NotEmpty().WithMessage("New scheduled date is required")
            .GreaterThanOrEqualTo(DateTime.Today)
            .WithMessage("New scheduled date must be today or in the future");

        RuleFor(x => x.Reason)
            .MaximumLength(500).WithMessage("Reason must not exceed 500 characters");
    }
}

