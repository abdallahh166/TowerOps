using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Signatures;

namespace TowerOps.Application.Queries.Signatures.GetVisitSignature;

public sealed record GetVisitSignatureQuery : IQuery<SignatureDto>
{
    public Guid VisitId { get; init; }
}
