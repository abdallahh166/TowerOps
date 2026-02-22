using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Signatures;

namespace TelecomPM.Application.Queries.Signatures.GetWorkOrderSignatures;

public sealed record GetWorkOrderSignaturesQuery : IQuery<WorkOrderSignaturesDto>
{
    public Guid WorkOrderId { get; init; }
}
