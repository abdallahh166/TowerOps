namespace TelecomPM.Application.Commands.Escalations.ApproveEscalation;

using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Escalations;

public record ApproveEscalationCommand : ICommand<EscalationDto>
{
    public Guid EscalationId { get; init; }
}
