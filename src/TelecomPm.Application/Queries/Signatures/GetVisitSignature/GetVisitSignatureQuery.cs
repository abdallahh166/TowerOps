using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Signatures;

namespace TelecomPM.Application.Queries.Signatures.GetVisitSignature;

public sealed record GetVisitSignatureQuery : IQuery<SignatureDto>
{
    public Guid VisitId { get; init; }
}
