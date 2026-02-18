namespace TelecomPM.Application.Commands.Visits.SubmitVisit;

using FluentValidation;

public class SubmitVisitCommandValidator : AbstractValidator<SubmitVisitCommand>
{
    public SubmitVisitCommandValidator()
    {
        RuleFor(x => x.VisitId)
            .NotEmpty().WithMessage("Visit ID is required");
    }
}