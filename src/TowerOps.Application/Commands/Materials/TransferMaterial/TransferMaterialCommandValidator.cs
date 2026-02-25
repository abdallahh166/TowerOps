namespace TowerOps.Application.Commands.Materials.TransferMaterial;

using FluentValidation;
using System;
using TowerOps.Domain.Enums;

public class TransferMaterialCommandValidator : AbstractValidator<TransferMaterialCommand>
{
    public TransferMaterialCommandValidator()
    {
        RuleFor(x => x.MaterialId)
            .NotEmpty().WithMessage("Material ID is required");

        RuleFor(x => x.FromOfficeId)
            .NotEmpty().WithMessage("Source office ID is required");

        RuleFor(x => x.ToOfficeId)
            .NotEmpty().WithMessage("Target office ID is required")
            .NotEqual(x => x.FromOfficeId).WithMessage("Source and target offices must be different");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than zero");

        RuleFor(x => x.Unit)
            .IsInEnum().WithMessage("Invalid material unit");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Transfer reason is required")
            .MaximumLength(500).WithMessage("Reason must not exceed 500 characters");

        RuleFor(x => x.TransferredBy)
            .NotEmpty().WithMessage("Transferred by is required");
    }
}

