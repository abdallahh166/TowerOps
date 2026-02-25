using FluentValidation;

namespace TowerOps.Application.Commands.DailyPlans.PublishDailyPlan;

public sealed class PublishDailyPlanCommandValidator : AbstractValidator<PublishDailyPlanCommand>
{
    public PublishDailyPlanCommandValidator()
    {
        RuleFor(x => x.PlanId).NotEmpty();
    }
}
