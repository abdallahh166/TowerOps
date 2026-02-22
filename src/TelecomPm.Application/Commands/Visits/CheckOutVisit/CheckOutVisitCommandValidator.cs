namespace TelecomPM.Application.Commands.Visits.CheckOutVisit;

using FluentValidation;

public sealed class CheckOutVisitCommandValidator : AbstractValidator<CheckOutVisitCommand>
{
    public CheckOutVisitCommandValidator()
    {
        RuleFor(x => x.VisitId).NotEmpty();
        RuleFor(x => x.EngineerId).NotEmpty();
        RuleFor(x => x.Latitude).InclusiveBetween(-90m, 90m);
        RuleFor(x => x.Longitude).InclusiveBetween(-180m, 180m);
    }
}
