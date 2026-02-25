namespace TowerOps.Application.Commands.Sites.UpdateSiteStatus;

using FluentValidation;
using System;
using TowerOps.Domain.Enums;

public class UpdateSiteStatusCommandValidator : AbstractValidator<UpdateSiteStatusCommand>
{
    public UpdateSiteStatusCommandValidator()
    {
        RuleFor(x => x.SiteId)
            .NotEmpty().WithMessage("Site ID is required");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Invalid site status");
    }
}

