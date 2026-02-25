namespace TowerOps.Application.Commands.WorkOrders.AssignWorkOrder;

using FluentValidation;

public class AssignWorkOrderCommandValidator : AbstractValidator<AssignWorkOrderCommand>
{
    public AssignWorkOrderCommandValidator()
    {
        RuleFor(x => x.WorkOrderId)
            .NotEmpty();

        RuleFor(x => x.EngineerId)
            .NotEmpty();

        RuleFor(x => x.EngineerName)
            .NotEmpty().MaximumLength(200);

        RuleFor(x => x.AssignedBy)
            .NotEmpty().MaximumLength(200);
    }
}
