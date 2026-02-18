namespace TelecomPM.Application.Commands.Materials.ReserveMaterial;

using FluentValidation;
using System;
using TelecomPM.Domain.Enums;

public class ReserveMaterialCommandValidator : AbstractValidator<ReserveMaterialCommand>
{
    public ReserveMaterialCommandValidator()
    {
        RuleFor(x => x.MaterialId)
            .NotEmpty().WithMessage("Material ID is required");

        RuleFor(x => x.VisitId)
            .NotEmpty().WithMessage("Visit ID is required");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than zero");

        RuleFor(x => x.Unit)
            .IsInEnum().WithMessage("Invalid material unit");
    }
}

