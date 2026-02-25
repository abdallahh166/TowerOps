namespace TowerOps.Application.Commands.Materials.ConsumeMaterial;

using FluentValidation;
using System;

public class ConsumeMaterialCommandValidator : AbstractValidator<ConsumeMaterialCommand>
{
    public ConsumeMaterialCommandValidator()
    {
        RuleFor(x => x.MaterialId)
            .NotEmpty().WithMessage("Material ID is required");

        RuleFor(x => x.VisitId)
            .NotEmpty().WithMessage("Visit ID is required");

        RuleFor(x => x.PerformedBy)
            .NotEmpty().WithMessage("Performed by is required");
    }
}

