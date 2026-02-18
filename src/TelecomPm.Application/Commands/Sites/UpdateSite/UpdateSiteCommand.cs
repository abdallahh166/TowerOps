namespace TelecomPM.Application.Commands.Sites.UpdateSite;

using System;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Sites;
using TelecomPM.Domain.Enums;

public record UpdateSiteCommand : ICommand<SiteDetailDto>
{
    public Guid SiteId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string OMCName { get; init; } = string.Empty;
    public SiteType SiteType { get; init; }
    public string? BSCName { get; init; }
    public string? BSCCode { get; init; }
    public string? Subcontractor { get; init; }
    public string? MaintenanceArea { get; init; }
}

