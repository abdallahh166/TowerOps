namespace TowerOps.Application.Commands.Materials.AdjustStock;

using FluentValidation;
using System;
using TowerOps.Domain.Enums;

public class AdjustStockCommandValidator : AbstractValidator<AdjustStockCommand>
{
    public AdjustStockCommandValidator()
    {
        RuleFor(x => x.MaterialId)
            .NotEmpty().WithMessage("Material ID is required");

        RuleFor(x => x.NewQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("New quantity must be zero or greater");

        RuleFor(x => x.Unit)
            .IsInEnum().WithMessage("Invalid material unit");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Adjustment reason is required")
            .MaximumLength(500).WithMessage("Reason must not exceed 500 characters");
    }
}

