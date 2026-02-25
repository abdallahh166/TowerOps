namespace TowerOps.Application.Commands.Visits.RejectVisit;

using FluentValidation;

public class RejectVisitCommandValidator : AbstractValidator<RejectVisitCommand>
{
    public RejectVisitCommandValidator()
    {
        RuleFor(x => x.VisitId)
            .NotEmpty().WithMessage("Visit ID is required");

        RuleFor(x => x.RejectionReason)
            .NotEmpty().WithMessage("Rejection reason is required")
            .MinimumLength(10).WithMessage("Rejection reason must be at least 10 characters")
            .MaximumLength(500).WithMessage("Rejection reason cannot exceed 500 characters");
    }
}
