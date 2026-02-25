namespace TowerOps.Api.Contracts.Sites;

using System.ComponentModel.DataAnnotations;
using TowerOps.Domain.Enums;

public record UpdateSiteStatusRequest
{
    [Required]
    public SiteStatus Status { get; init; }
}
