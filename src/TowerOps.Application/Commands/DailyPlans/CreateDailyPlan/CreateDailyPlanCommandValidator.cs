using FluentValidation;

namespace TowerOps.Application.Commands.DailyPlans.CreateDailyPlan;

public sealed class CreateDailyPlanCommandValidator : AbstractValidator<CreateDailyPlanCommand>
{
    public CreateDailyPlanCommandValidator()
    {
        RuleFor(x => x.OfficeId).NotEmpty();
        RuleFor(x => x.OfficeManagerId).NotEmpty();
        RuleFor(x => x.PlanDate).NotEqual(default(DateOnly));
    }
}
