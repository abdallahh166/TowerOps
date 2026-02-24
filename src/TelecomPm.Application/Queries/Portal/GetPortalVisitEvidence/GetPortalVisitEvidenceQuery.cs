using MediatR;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Portal;

namespace TelecomPM.Application.Queries.Portal.GetPortalVisitEvidence;

public sealed class GetPortalVisitEvidenceQuery : IRequest<Result<PortalVisitEvidenceDto>>
{
    public Guid VisitId { get; init; }
}
