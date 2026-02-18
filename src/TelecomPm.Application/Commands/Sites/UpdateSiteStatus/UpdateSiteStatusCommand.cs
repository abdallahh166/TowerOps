namespace TelecomPM.Application.Commands.Sites.UpdateSiteStatus;

using System;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Sites;
using TelecomPM.Domain.Enums;

public record UpdateSiteStatusCommand : ICommand<SiteDto>
{
    public Guid SiteId { get; init; }
    public SiteStatus Status { get; init; }
}

