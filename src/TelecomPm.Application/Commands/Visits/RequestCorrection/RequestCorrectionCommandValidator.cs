namespace TelecomPM.Application.Commands.Visits.RequestCorrection;

using FluentValidation;

public class RequestCorrectionCommandValidator : AbstractValidator<RequestCorrectionCommand>
{
    public RequestCorrectionCommandValidator()
    {
        RuleFor(x => x.VisitId)
            .NotEmpty().WithMessage("Visit ID is required");

        RuleFor(x => x.CorrectionNotes)
            .NotEmpty().WithMessage("Correction notes are required")
            .MinimumLength(10).WithMessage("Correction notes must be at least 10 characters")
            .MaximumLength(1000).WithMessage("Correction notes cannot exceed 1000 characters");
    }
}
