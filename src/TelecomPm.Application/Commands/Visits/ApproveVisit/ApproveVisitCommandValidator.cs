namespace TelecomPM.Application.Commands.Visits.ApproveVisit;

using FluentValidation;

public class ApproveVisitCommandValidator : AbstractValidator<ApproveVisitCommand>
{
    public ApproveVisitCommandValidator()
    {
        RuleFor(x => x.VisitId)
            .NotEmpty().WithMessage("Visit ID is required");

        RuleFor(x => x.Notes)
            .MaximumLength(500)
            .WithMessage("Notes cannot exceed 500 characters");
    }
}
