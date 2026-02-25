namespace TowerOps.Application.Commands.Escalations.CloseEscalation;

using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Escalations;

public record CloseEscalationCommand : ICommand<EscalationDto>
{
    public Guid EscalationId { get; init; }
}
