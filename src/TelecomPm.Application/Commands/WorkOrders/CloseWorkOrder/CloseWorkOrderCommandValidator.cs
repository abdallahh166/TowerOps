using FluentValidation;

namespace TelecomPM.Application.Commands.WorkOrders.CloseWorkOrder;

public class CloseWorkOrderCommandValidator : AbstractValidator<CloseWorkOrderCommand>
{
    public CloseWorkOrderCommandValidator()
    {
        RuleFor(x => x.WorkOrderId).NotEmpty();
    }
}
