namespace TowerOps.Application.Commands.Visits.StartVisit;

using FluentValidation;

public class StartVisitCommandValidator : AbstractValidator<StartVisitCommand>
{
    public StartVisitCommandValidator()
    {
        RuleFor(x => x.VisitId)
            .NotEmpty().WithMessage("Visit ID is required");

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90)
            .WithMessage("Latitude must be between -90 and 90");

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180)
            .WithMessage("Longitude must be between -180 and 180");
    }
}