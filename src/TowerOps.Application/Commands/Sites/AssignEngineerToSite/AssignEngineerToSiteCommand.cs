namespace TowerOps.Application.Commands.Sites.AssignEngineerToSite;

using System;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Sites;

public record AssignEngineerToSiteCommand : ICommand<SiteDto>
{
    public Guid SiteId { get; init; }
    public Guid EngineerId { get; init; }
    public Guid AssignedBy { get; init; }
}

