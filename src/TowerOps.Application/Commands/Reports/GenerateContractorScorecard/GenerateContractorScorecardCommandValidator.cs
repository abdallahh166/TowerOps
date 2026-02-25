using FluentValidation;

namespace TowerOps.Application.Commands.Reports.GenerateContractorScorecard;

public class GenerateContractorScorecardCommandValidator : AbstractValidator<GenerateContractorScorecardCommand>
{
    public GenerateContractorScorecardCommandValidator()
    {
        RuleFor(x => x.OfficeCode).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Month).InclusiveBetween(1, 12);
        RuleFor(x => x.Year).InclusiveBetween(2000, 2100);
    }
}
