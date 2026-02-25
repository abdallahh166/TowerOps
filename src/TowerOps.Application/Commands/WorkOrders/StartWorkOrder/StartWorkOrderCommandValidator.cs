using FluentValidation;

namespace TowerOps.Application.Commands.WorkOrders.StartWorkOrder;

public class StartWorkOrderCommandValidator : AbstractValidator<StartWorkOrderCommand>
{
    public StartWorkOrderCommandValidator()
    {
        RuleFor(x => x.WorkOrderId).NotEmpty();
    }
}
