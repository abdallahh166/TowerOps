namespace TowerOps.Application.Commands.WorkOrders.CreateWorkOrder;

using FluentValidation;

public class CreateWorkOrderCommandValidator : AbstractValidator<CreateWorkOrderCommand>
{
    public CreateWorkOrderCommandValidator()
    {
        RuleFor(x => x.WoNumber)
            .NotEmpty().WithMessage("WO number is required")
            .MaximumLength(50);

        RuleFor(x => x.SiteCode)
            .NotEmpty().WithMessage("Site code is required")
            .MaximumLength(50);

        RuleFor(x => x.OfficeCode)
            .NotEmpty().WithMessage("Office code is required")
            .MaximumLength(20);

        RuleFor(x => x.IssueDescription)
            .NotEmpty().WithMessage("Issue description is required")
            .MaximumLength(2000);
    }
}
