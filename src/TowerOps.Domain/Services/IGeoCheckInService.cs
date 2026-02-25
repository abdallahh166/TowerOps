using TowerOps.Domain.Entities.Visits;
using TowerOps.Domain.ValueObjects;

namespace TowerOps.Domain.Services;

public interface IGeoCheckInService
{
    GeoCheckInResult CheckIn(Visit visit, GeoLocation engineerLocation, GeoLocation siteLocation, decimal allowedRadiusMeters);
}

public sealed record GeoCheckInResult(decimal DistanceFromSiteMeters, bool IsWithinSiteRadius);
