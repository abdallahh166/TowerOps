namespace TowerOps.Application.Queries.Visits.GetVisitEvidenceStatus;

using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Visits;

public record GetVisitEvidenceStatusQuery : IQuery<VisitEvidenceStatusDto>
{
    public Guid VisitId { get; init; }
}
