using FluentValidation;

namespace TowerOps.Application.Commands.DailyPlans.RemoveSiteFromEngineer;

public sealed class RemoveSiteFromEngineerCommandValidator : AbstractValidator<RemoveSiteFromEngineerCommand>
{
    public RemoveSiteFromEngineerCommandValidator()
    {
        RuleFor(x => x.PlanId).NotEmpty();
        RuleFor(x => x.EngineerId).NotEmpty();
        RuleFor(x => x.SiteCode).NotEmpty();
    }
}
