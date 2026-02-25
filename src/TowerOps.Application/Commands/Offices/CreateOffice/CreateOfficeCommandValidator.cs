namespace TowerOps.Application.Commands.Offices.CreateOffice;

using FluentValidation;

public class CreateOfficeCommandValidator : AbstractValidator<CreateOfficeCommand>
{
    public CreateOfficeCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Office code is required")
            .Length(3).WithMessage("Office code must be exactly 3 characters")
            .Matches(@"^[A-Z]{3}$").WithMessage("Office code must be 3 uppercase letters");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Office name is required")
            .MaximumLength(200).WithMessage("Office name must not exceed 200 characters");

        RuleFor(x => x.Region)
            .NotEmpty().WithMessage("Region is required")
            .MaximumLength(100).WithMessage("Region must not exceed 100 characters");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City is required")
            .MaximumLength(100).WithMessage("City must not exceed 100 characters");

        RuleFor(x => x.Street)
            .NotEmpty().WithMessage("Street is required")
            .MaximumLength(200).WithMessage("Street must not exceed 200 characters");

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90")
            .When(x => x.Latitude.HasValue);

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180")
            .When(x => x.Longitude.HasValue);

        RuleFor(x => x.ContactEmail)
            .EmailAddress().WithMessage("Invalid email format")
            .When(x => !string.IsNullOrWhiteSpace(x.ContactEmail));
    }
}

