namespace TowerOps.Application.Commands.Visits.CancelVisit;

using FluentValidation;
using System;

public class CancelVisitCommandValidator : AbstractValidator<CancelVisitCommand>
{
    public CancelVisitCommandValidator()
    {
        RuleFor(x => x.VisitId)
            .NotEmpty().WithMessage("Visit ID is required");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Cancellation reason is required")
            .MaximumLength(500).WithMessage("Reason must not exceed 500 characters");
    }
}

