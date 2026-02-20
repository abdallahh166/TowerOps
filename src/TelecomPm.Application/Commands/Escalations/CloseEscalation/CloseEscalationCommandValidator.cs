namespace TelecomPM.Application.Commands.Escalations.CloseEscalation;

using FluentValidation;

public class CloseEscalationCommandValidator : AbstractValidator<CloseEscalationCommand>
{
    public CloseEscalationCommandValidator()
    {
        RuleFor(x => x.EscalationId).NotEmpty();
    }
}
