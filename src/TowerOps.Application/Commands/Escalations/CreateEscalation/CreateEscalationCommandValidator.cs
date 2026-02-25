namespace TowerOps.Application.Commands.Escalations.CreateEscalation;

using FluentValidation;

public class CreateEscalationCommandValidator : AbstractValidator<CreateEscalationCommand>
{
    public CreateEscalationCommandValidator()
    {
        RuleFor(x => x.WorkOrderId).NotEmpty();
        RuleFor(x => x.IncidentId).NotEmpty().MaximumLength(100);
        RuleFor(x => x.SiteCode).NotEmpty().MaximumLength(50);
        RuleFor(x => x.FinancialImpactEgp).GreaterThanOrEqualTo(0);
        RuleFor(x => x.SlaImpactPercentage).InclusiveBetween(0, 100);
        RuleFor(x => x.EvidencePackage).NotEmpty().MaximumLength(4000);
        RuleFor(x => x.PreviousActions).NotEmpty().MaximumLength(4000);
        RuleFor(x => x.RecommendedDecision).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.SubmittedBy).NotEmpty().MaximumLength(200);
    }
}
