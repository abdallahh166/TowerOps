using FluentValidation;

namespace TowerOps.Application.Commands.WorkOrders.SubmitForCustomerAcceptance;

public class SubmitForCustomerAcceptanceCommandValidator : AbstractValidator<SubmitForCustomerAcceptanceCommand>
{
    public SubmitForCustomerAcceptanceCommandValidator()
    {
        RuleFor(x => x.WorkOrderId).NotEmpty();
    }
}
