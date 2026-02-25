using MediatR;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Portal;

namespace TowerOps.Application.Queries.Portal.GetPortalVisitEvidence;

public sealed class GetPortalVisitEvidenceQuery : IRequest<Result<PortalVisitEvidenceDto>>
{
    public Guid VisitId { get; init; }
}
