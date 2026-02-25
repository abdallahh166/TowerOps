using FluentValidation;

namespace TowerOps.Application.Commands.WorkOrders.CancelWorkOrder;

public class CancelWorkOrderCommandValidator : AbstractValidator<CancelWorkOrderCommand>
{
    public CancelWorkOrderCommandValidator()
    {
        RuleFor(x => x.WorkOrderId).NotEmpty();
    }
}
