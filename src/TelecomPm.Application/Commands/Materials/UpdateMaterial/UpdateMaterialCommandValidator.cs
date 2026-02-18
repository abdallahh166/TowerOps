namespace TelecomPM.Application.Commands.Materials.UpdateMaterial;

using FluentValidation;
using System;
using TelecomPM.Domain.Enums;

public class UpdateMaterialCommandValidator : AbstractValidator<UpdateMaterialCommand>
{
    public UpdateMaterialCommandValidator()
    {
        RuleFor(x => x.MaterialId)
            .NotEmpty().WithMessage("Material ID is required");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Material name is required")
            .MaximumLength(200).WithMessage("Material name must not exceed 200 characters");

        RuleFor(x => x.Category)
            .IsInEnum().WithMessage("Invalid material category");
    }
}

