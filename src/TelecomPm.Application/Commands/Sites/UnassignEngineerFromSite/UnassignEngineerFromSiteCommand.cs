namespace TelecomPM.Application.Commands.Sites.UnassignEngineerFromSite;

using System;
using TelecomPM.Application.Common;

public record UnassignEngineerFromSiteCommand : ICommand
{
    public Guid SiteId { get; init; }
}

