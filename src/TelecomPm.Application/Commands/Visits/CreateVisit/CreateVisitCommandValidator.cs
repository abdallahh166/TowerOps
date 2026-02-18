namespace TelecomPM.Application.Commands.Visits.CreateVisit;

using FluentValidation;
using System;

public class CreateVisitCommandValidator : AbstractValidator<CreateVisitCommand>
{
    public CreateVisitCommandValidator()
    {
        RuleFor(x => x.SiteId)
            .NotEmpty().WithMessage("Site ID is required");

        RuleFor(x => x.EngineerId)
            .NotEmpty().WithMessage("Engineer ID is required");

        RuleFor(x => x.ScheduledDate)
            .NotEmpty().WithMessage("Scheduled date is required")
            .GreaterThanOrEqualTo(DateTime.Today)
            .WithMessage("Scheduled date must be today or in the future");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Invalid visit type");
    }
}