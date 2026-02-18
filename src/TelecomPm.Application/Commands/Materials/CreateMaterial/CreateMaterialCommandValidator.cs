namespace TelecomPM.Application.Commands.Materials.CreateMaterial;

using FluentValidation;
using System;
using TelecomPM.Domain.Enums;

public class CreateMaterialCommandValidator : AbstractValidator<CreateMaterialCommand>
{
    public CreateMaterialCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Material code is required")
            .MinimumLength(3).WithMessage("Material code must be at least 3 characters")
            .MaximumLength(20).WithMessage("Material code must not exceed 20 characters");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Material name is required")
            .MaximumLength(200).WithMessage("Material name must not exceed 200 characters");

        RuleFor(x => x.Category)
            .IsInEnum().WithMessage("Invalid material category");

        RuleFor(x => x.OfficeId)
            .NotEmpty().WithMessage("Office ID is required");

        RuleFor(x => x.InitialStock)
            .GreaterThan(0).WithMessage("Initial stock must be greater than zero");

        RuleFor(x => x.MinimumStock)
            .GreaterThanOrEqualTo(0).WithMessage("Minimum stock must be zero or greater");

        RuleFor(x => x.Unit)
            .IsInEnum().WithMessage("Invalid material unit");

        RuleFor(x => x.UnitCost)
            .GreaterThanOrEqualTo(0).WithMessage("Unit cost must be zero or greater");

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Currency is required")
            .Length(3).WithMessage("Currency must be a 3-letter code");
    }
}

