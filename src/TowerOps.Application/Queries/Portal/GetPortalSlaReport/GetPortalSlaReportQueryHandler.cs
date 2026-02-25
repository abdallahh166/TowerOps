using MediatR;
using TowerOps.Application.Common;
using TowerOps.Application.Common.Interfaces;
using TowerOps.Application.DTOs.Portal;
using TowerOps.Domain.Interfaces.Repositories;

namespace TowerOps.Application.Queries.Portal.GetPortalSlaReport;

public sealed class GetPortalSlaReportQueryHandler : IRequestHandler<GetPortalSlaReportQuery, Result<PortalSlaReportDto>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IUserRepository _userRepository;
    private readonly IPortalReadRepository _portalReadRepository;

    public GetPortalSlaReportQueryHandler(
        ICurrentUserService currentUserService,
        IUserRepository userRepository,
        IPortalReadRepository portalReadRepository)
    {
        _currentUserService = currentUserService;
        _userRepository = userRepository;
        _portalReadRepository = portalReadRepository;
    }

    public async Task<Result<PortalSlaReportDto>> Handle(GetPortalSlaReportQuery request, CancellationToken cancellationToken)
    {
        var portalUser = await _userRepository.GetByIdAsNoTrackingAsync(_currentUserService.UserId, cancellationToken);
        if (portalUser is null || !portalUser.IsClientPortalUser || string.IsNullOrWhiteSpace(portalUser.ClientCode))
            return Result.Failure<PortalSlaReportDto>("Portal access is not enabled for this user.");

        var report = await _portalReadRepository.GetSlaReportAsync(portalUser.ClientCode, cancellationToken);
        return Result.Success(report);
    }
}
