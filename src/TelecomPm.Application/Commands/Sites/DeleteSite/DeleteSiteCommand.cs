namespace TelecomPM.Application.Commands.Sites.DeleteSite;

using System;
using TelecomPM.Application.Common;

public record DeleteSiteCommand : ICommand
{
    public Guid SiteId { get; init; }
    public string DeletedBy { get; init; } = string.Empty;
}

