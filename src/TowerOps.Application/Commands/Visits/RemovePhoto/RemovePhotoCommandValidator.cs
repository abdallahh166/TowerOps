namespace TowerOps.Application.Commands.Visits.RemovePhoto;

using FluentValidation;
using System;

public class RemovePhotoCommandValidator : AbstractValidator<RemovePhotoCommand>
{
    public RemovePhotoCommandValidator()
    {
        RuleFor(x => x.VisitId)
            .NotEmpty().WithMessage("Visit ID is required");

        RuleFor(x => x.PhotoId)
            .NotEmpty().WithMessage("Photo ID is required");
    }
}

