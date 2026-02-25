namespace TowerOps.Api.Contracts.Visits;

using System.ComponentModel.DataAnnotations;

public record StartVisitRequest
{
    [Required]
    public double Latitude { get; init; }

    [Required]
    public double Longitude { get; init; }
}

