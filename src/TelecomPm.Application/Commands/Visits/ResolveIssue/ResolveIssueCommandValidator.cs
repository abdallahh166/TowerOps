namespace TelecomPM.Application.Commands.Visits.ResolveIssue;

using FluentValidation;
using System;

public class ResolveIssueCommandValidator : AbstractValidator<ResolveIssueCommand>
{
    public ResolveIssueCommandValidator()
    {
        RuleFor(x => x.VisitId)
            .NotEmpty().WithMessage("Visit ID is required");

        RuleFor(x => x.IssueId)
            .NotEmpty().WithMessage("Issue ID is required");

        RuleFor(x => x.Resolution)
            .NotEmpty().WithMessage("Resolution description is required")
            .MaximumLength(2000).WithMessage("Resolution must not exceed 2000 characters");
    }
}

