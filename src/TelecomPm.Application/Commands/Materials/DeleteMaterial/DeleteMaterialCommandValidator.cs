namespace TelecomPM.Application.Commands.Materials.DeleteMaterial;

using FluentValidation;
using System;

public class DeleteMaterialCommandValidator : AbstractValidator<DeleteMaterialCommand>
{
    public DeleteMaterialCommandValidator()
    {
        RuleFor(x => x.MaterialId)
            .NotEmpty().WithMessage("Material ID is required");

        RuleFor(x => x.DeletedBy)
            .NotEmpty().WithMessage("Deleted by is required");
    }
}

