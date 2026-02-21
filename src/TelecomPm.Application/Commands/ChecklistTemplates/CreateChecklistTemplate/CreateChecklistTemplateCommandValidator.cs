using FluentValidation;

namespace TelecomPM.Application.Commands.ChecklistTemplates.CreateChecklistTemplate;

public class CreateChecklistTemplateCommandValidator : AbstractValidator<CreateChecklistTemplateCommand>
{
    public CreateChecklistTemplateCommandValidator()
    {
        RuleFor(x => x.Version).NotEmpty().MaximumLength(20);
        RuleFor(x => x.EffectiveFromUtc).NotEmpty();
        RuleFor(x => x.CreatedBy).NotEmpty().MaximumLength(200);
        RuleForEach(x => x.Items).SetValidator(new CreateChecklistTemplateItemModelValidator());
    }
}

public class CreateChecklistTemplateItemModelValidator : AbstractValidator<CreateChecklistTemplateItemModel>
{
    public CreateChecklistTemplateItemModelValidator()
    {
        RuleFor(x => x.Category).NotEmpty().MaximumLength(100);
        RuleFor(x => x.ItemName).NotEmpty().MaximumLength(300);
    }
}
