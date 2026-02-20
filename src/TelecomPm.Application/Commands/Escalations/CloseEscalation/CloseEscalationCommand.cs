namespace TelecomPM.Application.Commands.Escalations.CloseEscalation;

using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Escalations;

public record CloseEscalationCommand : ICommand<EscalationDto>
{
    public Guid EscalationId { get; init; }
}
