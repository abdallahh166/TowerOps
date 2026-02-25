namespace TowerOps.Api.Contracts.Users;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public record AssignSitesRequest
{
    [Required]
    [MinLength(1, ErrorMessage = "At least one site ID is required")]
    public List<Guid> SiteIds { get; init; } = new();
}

