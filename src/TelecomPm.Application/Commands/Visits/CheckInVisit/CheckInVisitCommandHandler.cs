namespace TelecomPM.Application.Commands.Visits.CheckInVisit;

using MediatR;
using TelecomPM.Application.Common;
using TelecomPM.Application.Common.Interfaces;
using TelecomPM.Domain.Exceptions;
using TelecomPM.Domain.Interfaces.Repositories;
using TelecomPM.Domain.Services;
using TelecomPM.Domain.ValueObjects;

public sealed class CheckInVisitCommandHandler : IRequestHandler<CheckInVisitCommand, Result>
{
    private readonly IVisitRepository _visitRepository;
    private readonly ISiteRepository _siteRepository;
    private readonly IGeoCheckInService _geoCheckInService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ISystemSettingsService _settingsService;
    private readonly IUnitOfWork _unitOfWork;

    public CheckInVisitCommandHandler(
        IVisitRepository visitRepository,
        ISiteRepository siteRepository,
        IGeoCheckInService geoCheckInService,
        ICurrentUserService currentUserService,
        ISystemSettingsService settingsService,
        IUnitOfWork unitOfWork)
    {
        _visitRepository = visitRepository;
        _siteRepository = siteRepository;
        _geoCheckInService = geoCheckInService;
        _currentUserService = currentUserService;
        _settingsService = settingsService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CheckInVisitCommand request, CancellationToken cancellationToken)
    {
        var visit = await _visitRepository.GetByIdAsync(request.VisitId, cancellationToken);
        if (visit is null)
            return Result.Failure("Visit not found.");

        if (!_currentUserService.IsAuthenticated || _currentUserService.UserId == Guid.Empty)
            return Result.Failure("Authenticated user is required.");

        if (visit.EngineerId != _currentUserService.UserId)
            return Result.Failure("Only the assigned engineer can check in.");

        var site = await _siteRepository.GetByIdAsNoTrackingAsync(visit.SiteId, cancellationToken);
        if (site is null)
            return Result.Failure("Site not found.");

        try
        {
            var engineerLocation = GeoLocation.Create(request.Latitude, request.Longitude);
            var siteLocation = GeoLocation.Create((decimal)site.Coordinates.Latitude, (decimal)site.Coordinates.Longitude);
            var defaultRadius = await _settingsService.GetAsync("GPS:AllowedRadiusMeters", 200m, cancellationToken);
            var allowedRadius = site.AllowedCheckInRadiusMeters ?? defaultRadius;

            _geoCheckInService.CheckIn(visit, engineerLocation, siteLocation, allowedRadius);

            await _visitRepository.UpdateAsync(visit, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message);
        }
    }
}
