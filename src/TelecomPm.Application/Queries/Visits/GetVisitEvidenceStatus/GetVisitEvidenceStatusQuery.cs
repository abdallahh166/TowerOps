namespace TelecomPM.Application.Queries.Visits.GetVisitEvidenceStatus;

using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Visits;

public record GetVisitEvidenceStatusQuery : IQuery<VisitEvidenceStatusDto>
{
    public Guid VisitId { get; init; }
}
