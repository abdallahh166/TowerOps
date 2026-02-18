namespace TelecomPM.Application.Commands.Visits.UpdateChecklistItem;

using FluentValidation;
using System;
using TelecomPM.Domain.Enums;

public class UpdateChecklistItemCommandValidator : AbstractValidator<UpdateChecklistItemCommand>
{
    public UpdateChecklistItemCommandValidator()
    {
        RuleFor(x => x.VisitId)
            .NotEmpty().WithMessage("Visit ID is required");

        RuleFor(x => x.ChecklistItemId)
            .NotEmpty().WithMessage("Checklist item ID is required");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Invalid check status");

        RuleFor(x => x.Notes)
            .MaximumLength(500).WithMessage("Notes must not exceed 500 characters");
    }
}

