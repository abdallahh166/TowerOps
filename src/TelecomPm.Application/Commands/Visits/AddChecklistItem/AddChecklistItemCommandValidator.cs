namespace TelecomPM.Application.Commands.Visits.AddChecklistItem;

using FluentValidation;

public class AddChecklistItemCommandValidator : AbstractValidator<AddChecklistItemCommand>
{
    public AddChecklistItemCommandValidator()
    {
        RuleFor(x => x.VisitId).NotEmpty();
        RuleFor(x => x.Category).NotEmpty().MaximumLength(100);
        RuleFor(x => x.ItemName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(1000);
    }
}
