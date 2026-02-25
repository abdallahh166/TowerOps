namespace TowerOps.Application.Commands.Escalations.ApproveEscalation;

using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Escalations;

public record ApproveEscalationCommand : ICommand<EscalationDto>
{
    public Guid EscalationId { get; init; }
}
