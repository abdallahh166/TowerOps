namespace TelecomPm.Api.Contracts.Sites;

using System.ComponentModel.DataAnnotations;
using TelecomPM.Domain.Enums;

public record UpdateSiteRequest
{
    [StringLength(200)]
    public string? Name { get; init; }

    [StringLength(200)]
    public string? OMCName { get; init; }

    [StringLength(200)]
    public string? BSCName { get; init; }

    [StringLength(50)]
    public string? BSCCode { get; init; }

    [StringLength(100)]
    public string? Region { get; init; }

    [StringLength(100)]
    public string? SubRegion { get; init; }

    public SiteStatus? Status { get; init; }

    public SiteComplexity? Complexity { get; init; }
}

