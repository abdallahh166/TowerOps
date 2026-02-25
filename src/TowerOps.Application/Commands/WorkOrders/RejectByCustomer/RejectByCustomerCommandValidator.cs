using FluentValidation;

namespace TowerOps.Application.Commands.WorkOrders.RejectByCustomer;

public class RejectByCustomerCommandValidator : AbstractValidator<RejectByCustomerCommand>
{
    public RejectByCustomerCommandValidator()
    {
        RuleFor(x => x.WorkOrderId).NotEmpty();
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(2000);
    }
}
