namespace TowerOps.Application.Commands.Sites.UpdateSiteStatus;

using System;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Sites;
using TowerOps.Domain.Enums;

public record UpdateSiteStatusCommand : ICommand<SiteDto>
{
    public Guid SiteId { get; init; }
    public SiteStatus Status { get; init; }
}

