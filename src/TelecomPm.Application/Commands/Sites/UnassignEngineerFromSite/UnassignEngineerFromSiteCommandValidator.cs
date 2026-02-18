namespace TelecomPM.Application.Commands.Sites.UnassignEngineerFromSite;

using FluentValidation;
using System;

public class UnassignEngineerFromSiteCommandValidator : AbstractValidator<UnassignEngineerFromSiteCommand>
{
    public UnassignEngineerFromSiteCommandValidator()
    {
        RuleFor(x => x.SiteId)
            .NotEmpty().WithMessage("Site ID is required");
    }
}

