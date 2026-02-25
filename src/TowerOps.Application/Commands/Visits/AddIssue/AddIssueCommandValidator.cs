namespace TowerOps.Application.Commands.Visits.AddIssue;

using FluentValidation;
using System;
using TowerOps.Domain.Enums;

public class AddIssueCommandValidator : AbstractValidator<AddIssueCommand>
{
    public AddIssueCommandValidator()
    {
        RuleFor(x => x.VisitId)
            .NotEmpty().WithMessage("Visit ID is required");

        RuleFor(x => x.Category)
            .IsInEnum().WithMessage("Invalid issue category");

        RuleFor(x => x.Severity)
            .IsInEnum().WithMessage("Invalid issue severity");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Issue title is required")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Issue description is required")
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters");
    }
}

