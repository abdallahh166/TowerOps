using FluentValidation;

namespace TowerOps.Application.Commands.Sites.ImportSiteData;

public class ImportSiteDataCommandValidator : AbstractValidator<ImportSiteDataCommand>
{
    public ImportSiteDataCommandValidator()
    {
        RuleFor(x => x.FileContent)
            .NotNull()
            .Must(x => x.Length > 0)
            .WithMessage("Excel file content is required.");
    }
}
