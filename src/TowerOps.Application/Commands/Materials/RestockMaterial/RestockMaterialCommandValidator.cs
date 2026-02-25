namespace TowerOps.Application.Commands.Materials.RestockMaterial;

using FluentValidation;
using System;
using TowerOps.Domain.Enums;

public class RestockMaterialCommandValidator : AbstractValidator<RestockMaterialCommand>
{
    public RestockMaterialCommandValidator()
    {
        RuleFor(x => x.MaterialId)
            .NotEmpty().WithMessage("Material ID is required");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than zero");

        RuleFor(x => x.Unit)
            .IsInEnum().WithMessage("Invalid material unit");

        RuleFor(x => x.RestockedBy)
            .NotEmpty().WithMessage("Restocked by is required");
    }
}

