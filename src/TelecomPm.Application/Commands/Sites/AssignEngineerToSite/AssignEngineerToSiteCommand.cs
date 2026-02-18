namespace TelecomPM.Application.Commands.Sites.AssignEngineerToSite;

using System;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Sites;

public record AssignEngineerToSiteCommand : ICommand<SiteDto>
{
    public Guid SiteId { get; init; }
    public Guid EngineerId { get; init; }
    public Guid AssignedBy { get; init; }
}

