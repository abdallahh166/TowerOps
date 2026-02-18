namespace TelecomPM.Application.Commands.Sites.UpdateSite;

using FluentValidation;
using System;
using TelecomPM.Domain.Enums;

public class UpdateSiteCommandValidator : AbstractValidator<UpdateSiteCommand>
{
    public UpdateSiteCommandValidator()
    {
        RuleFor(x => x.SiteId)
            .NotEmpty().WithMessage("Site ID is required");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Site name is required")
            .MaximumLength(200).WithMessage("Site name must not exceed 200 characters");

        RuleFor(x => x.OMCName)
            .NotEmpty().WithMessage("OMC name is required")
            .MaximumLength(200).WithMessage("OMC name must not exceed 200 characters");

        RuleFor(x => x.SiteType)
            .IsInEnum().WithMessage("Invalid site type");
    }
}

