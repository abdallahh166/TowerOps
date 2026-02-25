namespace TowerOps.Application.DTOs.Sites;

public sealed record SiteLocationDto
{
    public string SiteCode { get; init; } = string.Empty;
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public decimal AllowedRadiusMeters { get; init; }
}
