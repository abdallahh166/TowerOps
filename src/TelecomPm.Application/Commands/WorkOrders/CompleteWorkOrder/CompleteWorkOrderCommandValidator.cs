using FluentValidation;

namespace TelecomPM.Application.Commands.WorkOrders.CompleteWorkOrder;

public class CompleteWorkOrderCommandValidator : AbstractValidator<CompleteWorkOrderCommand>
{
    public CompleteWorkOrderCommandValidator()
    {
        RuleFor(x => x.WorkOrderId).NotEmpty();
    }
}
