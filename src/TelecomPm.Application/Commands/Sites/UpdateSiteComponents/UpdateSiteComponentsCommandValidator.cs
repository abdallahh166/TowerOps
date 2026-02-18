namespace TelecomPM.Application.Commands.Sites.UpdateSiteComponents;

using FluentValidation;
using System;

public class UpdateSiteComponentsCommandValidator : AbstractValidator<UpdateSiteComponentsCommand>
{
    public UpdateSiteComponentsCommandValidator()
    {
        RuleFor(x => x.SiteId)
            .NotEmpty().WithMessage("Site ID is required");

        // At least one component should be provided
        RuleFor(x => x)
            .Must(x => x.TowerInfo != null || x.PowerSystem != null || x.RadioEquipment != null ||
                      x.Transmission != null || x.CoolingSystem != null || x.FireSafety != null)
            .WithMessage("At least one component must be provided for update");
    }
}

