using FluentValidation;

namespace TelecomPM.Application.Commands.WorkOrders.RejectByCustomer;

public class RejectByCustomerCommandValidator : AbstractValidator<RejectByCustomerCommand>
{
    public RejectByCustomerCommandValidator()
    {
        RuleFor(x => x.WorkOrderId).NotEmpty();
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(2000);
    }
}
