using FluentValidation;

namespace TelecomPM.Application.Commands.Settings.UpsertSystemSettings;

public sealed class UpsertSystemSettingsCommandValidator : AbstractValidator<UpsertSystemSettingsCommand>
{
    public UpsertSystemSettingsCommandValidator()
    {
        RuleFor(x => x.Settings)
            .NotEmpty()
            .WithMessage("At least one setting is required.");

        RuleForEach(x => x.Settings).ChildRules(setting =>
        {
            setting.RuleFor(x => x.Key)
                .NotEmpty()
                .WithMessage("Setting key is required.");

            setting.RuleFor(x => x.Key)
                .MaximumLength(200)
                .WithMessage("Setting key must not exceed 200 characters.");

            setting.RuleFor(x => x.Group)
                .MaximumLength(100)
                .WithMessage("Group must not exceed 100 characters.");

            setting.RuleFor(x => x.DataType)
                .MaximumLength(50)
                .WithMessage("Data type must not exceed 50 characters.");
        });
    }
}
