namespace TowerOps.Application.Commands.Sites.UnassignEngineerFromSite;

using System;
using TowerOps.Application.Common;

public record UnassignEngineerFromSiteCommand : ICommand
{
    public Guid SiteId { get; init; }
}

