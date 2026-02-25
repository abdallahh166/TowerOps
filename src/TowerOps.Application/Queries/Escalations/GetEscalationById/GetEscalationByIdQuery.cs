namespace TowerOps.Application.Queries.Escalations.GetEscalationById;

using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Escalations;

public record GetEscalationByIdQuery : IQuery<EscalationDto>
{
    public Guid EscalationId { get; init; }
}
