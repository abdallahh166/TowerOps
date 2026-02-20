namespace TelecomPm.Api.Contracts.Sites;

using System.ComponentModel.DataAnnotations;
using TelecomPM.Domain.Enums;

public record UpdateSiteStatusRequest
{
    [Required]
    public SiteStatus Status { get; init; }
}
