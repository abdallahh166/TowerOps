using FluentValidation;
using System.Linq;
using System;

namespace TowerOps.Application.Commands.Visits.AddPhoto;

public class AddPhotoCommandValidator : AbstractValidator<AddPhotoCommand>
{
    public AddPhotoCommandValidator()
    {
        RuleFor(x => x.VisitId)
            .NotEmpty().WithMessage("Visit ID is required");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Invalid photo type");

        RuleFor(x => x.Category)
            .IsInEnum().WithMessage("Invalid photo category");

        RuleFor(x => x.ItemName)
            .NotEmpty().WithMessage("Item name is required")
            .MaximumLength(200).WithMessage("Item name cannot exceed 200 characters");

        RuleFor(x => x.FileName)
            .NotEmpty().WithMessage("File name is required")
            .Must(BeAValidFileName).WithMessage("Invalid file name");

        RuleFor(x => x.FileStream)
            .NotNull().WithMessage("File stream is required");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");

        When(x => x.Latitude.HasValue, () =>
        {
            RuleFor(x => x.Latitude)
                .InclusiveBetween(-90, 90)
                .WithMessage("Latitude must be between -90 and 90");
        });

        When(x => x.Longitude.HasValue, () =>
        {
            RuleFor(x => x.Longitude)
                .InclusiveBetween(-180, 180)
                .WithMessage("Longitude must be between -180 and 180");
        });
    }

    private bool BeAValidFileName(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return false;

        var validExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf" };
        return validExtensions.Any(ext => fileName.EndsWith(ext, StringComparison.OrdinalIgnoreCase));
    }
}
