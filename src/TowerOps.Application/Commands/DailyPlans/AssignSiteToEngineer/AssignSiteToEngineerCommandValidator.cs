using FluentValidation;

namespace TowerOps.Application.Commands.DailyPlans.AssignSiteToEngineer;

public sealed class AssignSiteToEngineerCommandValidator : AbstractValidator<AssignSiteToEngineerCommand>
{
    public AssignSiteToEngineerCommandValidator()
    {
        RuleFor(x => x.PlanId).NotEmpty();
        RuleFor(x => x.EngineerId).NotEmpty();
        RuleFor(x => x.SiteCode).NotEmpty();
        RuleFor(x => x.Priority).NotEmpty().MaximumLength(16);
    }
}
