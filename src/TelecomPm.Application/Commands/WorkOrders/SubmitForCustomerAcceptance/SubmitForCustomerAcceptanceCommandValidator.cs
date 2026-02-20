using FluentValidation;

namespace TelecomPM.Application.Commands.WorkOrders.SubmitForCustomerAcceptance;

public class SubmitForCustomerAcceptanceCommandValidator : AbstractValidator<SubmitForCustomerAcceptanceCommand>
{
    public SubmitForCustomerAcceptanceCommandValidator()
    {
        RuleFor(x => x.WorkOrderId).NotEmpty();
    }
}
