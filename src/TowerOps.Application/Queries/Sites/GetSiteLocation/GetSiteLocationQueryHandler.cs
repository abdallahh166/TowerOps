namespace TowerOps.Application.Queries.Sites.GetSiteLocation;

using MediatR;
using TowerOps.Application.Common;
using TowerOps.Application.Common.Interfaces;
using TowerOps.Application.DTOs.Sites;
using TowerOps.Domain.Interfaces.Repositories;

public sealed class GetSiteLocationQueryHandler : IRequestHandler<GetSiteLocationQuery, Result<SiteLocationDto>>
{
    private readonly ISiteRepository _siteRepository;
    private readonly ISystemSettingsService _settingsService;

    public GetSiteLocationQueryHandler(ISiteRepository siteRepository, ISystemSettingsService settingsService)
    {
        _siteRepository = siteRepository;
        _settingsService = settingsService;
    }

    public async Task<Result<SiteLocationDto>> Handle(GetSiteLocationQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.SiteCode))
            return Result.Failure<SiteLocationDto>("Site code is required.");

        var site = await _siteRepository.GetBySiteCodeAsNoTrackingAsync(request.SiteCode.Trim(), cancellationToken);
        if (site is null)
            return Result.Failure<SiteLocationDto>("Site not found.");

        var defaultRadius = await _settingsService.GetAsync("GPS:AllowedRadiusMeters", 200m, cancellationToken);

        return Result.Success(new SiteLocationDto
        {
            SiteCode = site.SiteCode.Value,
            Latitude = site.Coordinates.Latitude,
            Longitude = site.Coordinates.Longitude,
            AllowedRadiusMeters = site.AllowedCheckInRadiusMeters ?? defaultRadius
        });
    }
}
