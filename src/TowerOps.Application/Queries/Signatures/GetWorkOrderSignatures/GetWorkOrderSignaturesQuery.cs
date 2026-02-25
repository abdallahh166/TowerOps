using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Signatures;

namespace TowerOps.Application.Queries.Signatures.GetWorkOrderSignatures;

public sealed record GetWorkOrderSignaturesQuery : IQuery<WorkOrderSignaturesDto>
{
    public Guid WorkOrderId { get; init; }
}
