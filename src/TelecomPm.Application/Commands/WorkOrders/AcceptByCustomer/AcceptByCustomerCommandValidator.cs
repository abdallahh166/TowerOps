using FluentValidation;

namespace TelecomPM.Application.Commands.WorkOrders.AcceptByCustomer;

public class AcceptByCustomerCommandValidator : AbstractValidator<AcceptByCustomerCommand>
{
    public AcceptByCustomerCommandValidator()
    {
        RuleFor(x => x.WorkOrderId).NotEmpty();
        RuleFor(x => x.AcceptedBy).NotEmpty().MaximumLength(200);
    }
}
