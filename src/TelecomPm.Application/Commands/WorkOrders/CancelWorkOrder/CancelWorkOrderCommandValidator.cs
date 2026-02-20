using FluentValidation;

namespace TelecomPM.Application.Commands.WorkOrders.CancelWorkOrder;

public class CancelWorkOrderCommandValidator : AbstractValidator<CancelWorkOrderCommand>
{
    public CancelWorkOrderCommandValidator()
    {
        RuleFor(x => x.WorkOrderId).NotEmpty();
    }
}
