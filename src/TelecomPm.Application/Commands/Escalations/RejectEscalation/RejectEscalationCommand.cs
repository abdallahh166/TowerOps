namespace TelecomPM.Application.Commands.Escalations.RejectEscalation;

using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Escalations;

public record RejectEscalationCommand : ICommand<EscalationDto>
{
    public Guid EscalationId { get; init; }
}
