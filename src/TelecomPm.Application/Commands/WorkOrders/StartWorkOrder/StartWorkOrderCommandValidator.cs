using FluentValidation;

namespace TelecomPM.Application.Commands.WorkOrders.StartWorkOrder;

public class StartWorkOrderCommandValidator : AbstractValidator<StartWorkOrderCommand>
{
    public StartWorkOrderCommandValidator()
    {
        RuleFor(x => x.WorkOrderId).NotEmpty();
    }
}
