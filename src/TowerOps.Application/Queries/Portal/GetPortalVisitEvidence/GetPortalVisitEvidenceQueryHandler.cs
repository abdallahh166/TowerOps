using MediatR;
using TowerOps.Application.Common;
using TowerOps.Application.Common.Interfaces;
using TowerOps.Application.DTOs.Portal;
using TowerOps.Domain.Interfaces.Repositories;

namespace TowerOps.Application.Queries.Portal.GetPortalVisitEvidence;

public sealed class GetPortalVisitEvidenceQueryHandler : IRequestHandler<GetPortalVisitEvidenceQuery, Result<PortalVisitEvidenceDto>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IUserRepository _userRepository;
    private readonly IPortalReadRepository _portalReadRepository;

    public GetPortalVisitEvidenceQueryHandler(
        ICurrentUserService currentUserService,
        IUserRepository userRepository,
        IPortalReadRepository portalReadRepository)
    {
        _currentUserService = currentUserService;
        _userRepository = userRepository;
        _portalReadRepository = portalReadRepository;
    }

    public async Task<Result<PortalVisitEvidenceDto>> Handle(GetPortalVisitEvidenceQuery request, CancellationToken cancellationToken)
    {
        if (request.VisitId == Guid.Empty)
            return Result.Failure<PortalVisitEvidenceDto>("VisitId is required.");

        var portalUser = await _userRepository.GetByIdAsNoTrackingAsync(_currentUserService.UserId, cancellationToken);
        if (portalUser is null || !portalUser.IsClientPortalUser || string.IsNullOrWhiteSpace(portalUser.ClientCode))
            return Result.Failure<PortalVisitEvidenceDto>("Portal access is not enabled for this user.");

        var evidence = await _portalReadRepository.GetVisitEvidenceAsync(
            portalUser.ClientCode,
            request.VisitId,
            cancellationToken);

        if (evidence is null)
            return Result.Failure<PortalVisitEvidenceDto>("Visit not found.");

        return Result.Success(evidence);
    }
}
