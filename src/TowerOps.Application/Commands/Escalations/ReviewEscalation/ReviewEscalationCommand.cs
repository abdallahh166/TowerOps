namespace TowerOps.Application.Commands.Escalations.ReviewEscalation;

using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Escalations;

public record ReviewEscalationCommand : ICommand<EscalationDto>
{
    public Guid EscalationId { get; init; }
}
