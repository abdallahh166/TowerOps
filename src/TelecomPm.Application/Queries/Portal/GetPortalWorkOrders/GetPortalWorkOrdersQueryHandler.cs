using MediatR;
using TelecomPM.Application.Common;
using TelecomPM.Application.Common.Interfaces;
using TelecomPM.Application.DTOs.Portal;
using TelecomPM.Domain.Interfaces.Repositories;

namespace TelecomPM.Application.Queries.Portal.GetPortalWorkOrders;

public sealed class GetPortalWorkOrdersQueryHandler : IRequestHandler<GetPortalWorkOrdersQuery, Result<IReadOnlyList<PortalWorkOrderDto>>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IUserRepository _userRepository;
    private readonly IPortalReadRepository _portalReadRepository;

    public GetPortalWorkOrdersQueryHandler(
        ICurrentUserService currentUserService,
        IUserRepository userRepository,
        IPortalReadRepository portalReadRepository)
    {
        _currentUserService = currentUserService;
        _userRepository = userRepository;
        _portalReadRepository = portalReadRepository;
    }

    public async Task<Result<IReadOnlyList<PortalWorkOrderDto>>> Handle(GetPortalWorkOrdersQuery request, CancellationToken cancellationToken)
    {
        var portalUser = await _userRepository.GetByIdAsNoTrackingAsync(_currentUserService.UserId, cancellationToken);
        if (portalUser is null || !portalUser.IsClientPortalUser || string.IsNullOrWhiteSpace(portalUser.ClientCode))
            return Result.Failure<IReadOnlyList<PortalWorkOrderDto>>("Portal access is not enabled for this user.");

        var safePageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var safePageSize = Math.Clamp(request.PageSize, 1, 200);

        var workOrders = await _portalReadRepository.GetWorkOrdersAsync(
            portalUser.ClientCode,
            safePageNumber,
            safePageSize,
            cancellationToken);

        return Result.Success(workOrders);
    }
}
