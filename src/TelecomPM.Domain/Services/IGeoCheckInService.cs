using TelecomPM.Domain.Entities.Visits;
using TelecomPM.Domain.ValueObjects;

namespace TelecomPM.Domain.Services;

public interface IGeoCheckInService
{
    GeoCheckInResult CheckIn(Visit visit, GeoLocation engineerLocation, GeoLocation siteLocation, decimal allowedRadiusMeters);
}

public sealed record GeoCheckInResult(decimal DistanceFromSiteMeters, bool IsWithinSiteRadius);
