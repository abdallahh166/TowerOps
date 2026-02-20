namespace TelecomPM.Application.Commands.Escalations.RejectEscalation;

using FluentValidation;

public class RejectEscalationCommandValidator : AbstractValidator<RejectEscalationCommand>
{
    public RejectEscalationCommandValidator()
    {
        RuleFor(x => x.EscalationId).NotEmpty();
    }
}
