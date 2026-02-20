namespace TelecomPM.Application.Commands.Escalations.ReviewEscalation;

using FluentValidation;

public class ReviewEscalationCommandValidator : AbstractValidator<ReviewEscalationCommand>
{
    public ReviewEscalationCommandValidator()
    {
        RuleFor(x => x.EscalationId).NotEmpty();
    }
}
