namespace TelecomPM.Application.Commands.Visits.CheckInVisit;

using FluentValidation;

public sealed class CheckInVisitCommandValidator : AbstractValidator<CheckInVisitCommand>
{
    public CheckInVisitCommandValidator()
    {
        RuleFor(x => x.VisitId).NotEmpty();
        RuleFor(x => x.Latitude).InclusiveBetween(-90m, 90m);
        RuleFor(x => x.Longitude).InclusiveBetween(-180m, 180m);
    }
}
