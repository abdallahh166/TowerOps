using System.Collections.Generic;
using TelecomPM.Domain.Exceptions;

namespace TelecomPM.Domain.ValueObjects;

// ==================== Address ====================
public sealed class Address : ValueObject
{
    public string Street { get; }
    public string City { get; }
    public string Region { get; }
    public string Details { get; }

    private Address(string street, string city, string region, string details)
    {
        Street = street;
        City = city;
        Region = region;
        Details = details;
    }

    public static Address Create(string street, string city, string region, string details = "")
    {
        if (string.IsNullOrWhiteSpace(city))
            throw new DomainException("City is required");

        if (string.IsNullOrWhiteSpace(region))
            throw new DomainException("Region is required");

        return new Address(street ?? "", city, region, details ?? "");
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Street;
        yield return City;
        yield return Region;
    }

    public override string ToString() => $"{Street}, {City}, {Region}";
}