namespace TelecomPM.Application.Commands.Escalations.ReviewEscalation;

using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Escalations;

public record ReviewEscalationCommand : ICommand<EscalationDto>
{
    public Guid EscalationId { get; init; }
}
