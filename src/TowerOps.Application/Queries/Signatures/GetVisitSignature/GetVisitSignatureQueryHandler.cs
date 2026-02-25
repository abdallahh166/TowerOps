using MediatR;
using TowerOps.Application.Commands.Signatures;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Signatures;
using TowerOps.Domain.Interfaces.Repositories;

namespace TowerOps.Application.Queries.Signatures.GetVisitSignature;

public sealed class GetVisitSignatureQueryHandler : IRequestHandler<GetVisitSignatureQuery, Result<SignatureDto>>
{
    private readonly IVisitRepository _visitRepository;

    public GetVisitSignatureQueryHandler(IVisitRepository visitRepository)
    {
        _visitRepository = visitRepository;
    }

    public async Task<Result<SignatureDto>> Handle(GetVisitSignatureQuery request, CancellationToken cancellationToken)
    {
        var visit = await _visitRepository.GetByIdAsNoTrackingAsync(request.VisitId, cancellationToken);
        if (visit is null)
            return Result.Failure<SignatureDto>("Visit not found.");

        if (visit.SiteContactSignature is null)
            return Result.Failure<SignatureDto>("Visit signature not found.");

        return Result.Success(SignatureMapper.ToDto(visit.SiteContactSignature));
    }
}
