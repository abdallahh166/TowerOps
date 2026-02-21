using FluentValidation;

namespace TelecomPM.Application.Commands.ChecklistTemplates.ActivateChecklistTemplate;

public class ActivateChecklistTemplateCommandValidator : AbstractValidator<ActivateChecklistTemplateCommand>
{
    public ActivateChecklistTemplateCommandValidator()
    {
        RuleFor(x => x.TemplateId).NotEmpty();
        RuleFor(x => x.ApprovedBy).NotEmpty().MaximumLength(200);
    }
}
