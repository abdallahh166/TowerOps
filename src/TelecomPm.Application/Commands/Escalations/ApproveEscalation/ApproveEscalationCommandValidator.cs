namespace TelecomPM.Application.Commands.Escalations.ApproveEscalation;

using FluentValidation;

public class ApproveEscalationCommandValidator : AbstractValidator<ApproveEscalationCommand>
{
    public ApproveEscalationCommandValidator()
    {
        RuleFor(x => x.EscalationId).NotEmpty();
    }
}
