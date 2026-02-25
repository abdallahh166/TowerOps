namespace TowerOps.Application.Commands.Escalations.RejectEscalation;

using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Escalations;

public record RejectEscalationCommand : ICommand<EscalationDto>
{
    public Guid EscalationId { get; init; }
}
