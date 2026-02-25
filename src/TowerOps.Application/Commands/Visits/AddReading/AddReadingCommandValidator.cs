namespace TowerOps.Application.Commands.Visits.AddReading;

using FluentValidation;

public class AddReadingCommandValidator : AbstractValidator<AddReadingCommand>
{
    public AddReadingCommandValidator()
    {
        RuleFor(x => x.VisitId)
            .NotEmpty().WithMessage("Visit ID is required");

        RuleFor(x => x.ReadingType)
            .NotEmpty().WithMessage("Reading type is required")
            .MaximumLength(100).WithMessage("Reading type cannot exceed 100 characters");

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("Category is required")
            .MaximumLength(100).WithMessage("Category cannot exceed 100 characters");

        RuleFor(x => x.Value)
            .NotEmpty().WithMessage("Value is required");

        RuleFor(x => x.Unit)
            .NotEmpty().WithMessage("Unit is required")
            .MaximumLength(20).WithMessage("Unit cannot exceed 20 characters");

        RuleFor(x => x.Phase)
            .MaximumLength(10).WithMessage("Phase cannot exceed 10 characters");

        RuleFor(x => x.Equipment)
            .MaximumLength(100).WithMessage("Equipment cannot exceed 100 characters");

        RuleFor(x => x.Notes)
            .MaximumLength(500).WithMessage("Notes cannot exceed 500 characters");

        When(x => x.MinAcceptable.HasValue && x.MaxAcceptable.HasValue, () =>
        {
            RuleFor(x => x.MinAcceptable)
                .LessThan(x => x.MaxAcceptable)
                .WithMessage("Minimum acceptable must be less than maximum acceptable");
        });
    }
}