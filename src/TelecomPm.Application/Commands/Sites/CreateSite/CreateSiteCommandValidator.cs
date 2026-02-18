namespace TelecomPM.Application.Commands.Sites.CreateSite;

using FluentValidation;
using System;
using TelecomPM.Domain.Enums;

public class CreateSiteCommandValidator : AbstractValidator<CreateSiteCommand>
{
    public CreateSiteCommandValidator()
    {
        RuleFor(x => x.SiteCode)
            .NotEmpty().WithMessage("Site code is required")
            .MaximumLength(50).WithMessage("Site code must not exceed 50 characters");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Site name is required")
            .MaximumLength(200).WithMessage("Site name must not exceed 200 characters");

        RuleFor(x => x.OMCName)
            .NotEmpty().WithMessage("OMC name is required")
            .MaximumLength(200).WithMessage("OMC name must not exceed 200 characters");

        RuleFor(x => x.OfficeId)
            .NotEmpty().WithMessage("Office ID is required");

        RuleFor(x => x.Region)
            .NotEmpty().WithMessage("Region is required")
            .MaximumLength(100).WithMessage("Region must not exceed 100 characters");

        RuleFor(x => x.SubRegion)
            .NotEmpty().WithMessage("Sub-region is required")
            .MaximumLength(100).WithMessage("Sub-region must not exceed 100 characters");

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90");

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City is required")
            .MaximumLength(100).WithMessage("City must not exceed 100 characters");

        RuleFor(x => x.AddressRegion)
            .NotEmpty().WithMessage("Address region is required")
            .MaximumLength(100).WithMessage("Address region must not exceed 100 characters");

        RuleFor(x => x.SiteType)
            .IsInEnum().WithMessage("Invalid site type");
    }
}

