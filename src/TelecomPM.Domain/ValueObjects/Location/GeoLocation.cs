using TelecomPM.Domain.Exceptions;

namespace TelecomPM.Domain.ValueObjects;

public sealed class GeoLocation : ValueObject
{
    public decimal Latitude { get; }
    public decimal Longitude { get; }

    private GeoLocation(decimal latitude, decimal longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }

    public static GeoLocation Create(decimal latitude, decimal longitude)
    {
        if (latitude < -90m || latitude > 90m)
            throw new DomainException("Latitude must be between -90 and 90");

        if (longitude < -180m || longitude > 180m)
            throw new DomainException("Longitude must be between -180 and 180");

        return new GeoLocation(latitude, longitude);
    }

    public decimal DistanceTo(GeoLocation other)
    {
        const double earthRadiusMeters = 6371000d;
        var lat1 = ToRadians((double)Latitude);
        var lat2 = ToRadians((double)other.Latitude);
        var deltaLat = ToRadians((double)(other.Latitude - Latitude));
        var deltaLon = ToRadians((double)(other.Longitude - Longitude));

        var a = Math.Sin(deltaLat / 2d) * Math.Sin(deltaLat / 2d) +
                Math.Cos(lat1) * Math.Cos(lat2) *
                Math.Sin(deltaLon / 2d) * Math.Sin(deltaLon / 2d);

        var c = 2d * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1d - a));
        var meters = earthRadiusMeters * c;
        return decimal.Round((decimal)meters, 2, MidpointRounding.AwayFromZero);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Latitude;
        yield return Longitude;
    }

    private static double ToRadians(double degrees) => degrees * (Math.PI / 180d);
}
