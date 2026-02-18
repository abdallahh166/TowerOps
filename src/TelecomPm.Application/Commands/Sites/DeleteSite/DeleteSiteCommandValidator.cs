namespace TelecomPM.Application.Commands.Sites.DeleteSite;

using FluentValidation;
using System;

public class DeleteSiteCommandValidator : AbstractValidator<DeleteSiteCommand>
{
    public DeleteSiteCommandValidator()
    {
        RuleFor(x => x.SiteId)
            .NotEmpty().WithMessage("Site ID is required");

        RuleFor(x => x.DeletedBy)
            .NotEmpty().WithMessage("DeletedBy is required");
    }
}

