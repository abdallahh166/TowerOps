namespace TelecomPM.Application.Commands.Sites.AssignEngineerToSite;

using FluentValidation;
using System;

public class AssignEngineerToSiteCommandValidator : AbstractValidator<AssignEngineerToSiteCommand>
{
    public AssignEngineerToSiteCommandValidator()
    {
        RuleFor(x => x.SiteId)
            .NotEmpty().WithMessage("Site ID is required");

        RuleFor(x => x.EngineerId)
            .NotEmpty().WithMessage("Engineer ID is required");

        RuleFor(x => x.AssignedBy)
            .NotEmpty().WithMessage("AssignedBy is required");
    }
}

