namespace TelecomPM.Application.Commands.Sites.CreateSite;

using System;
using System.Collections.Generic;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Sites;
using TelecomPM.Domain.Enums;

public record CreateSiteCommand : ICommand<SiteDetailDto>
{
    public string SiteCode { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string OMCName { get; init; } = string.Empty;
    public Guid OfficeId { get; init; }
    public string Region { get; init; } = string.Empty;
    public string SubRegion { get; init; } = string.Empty;
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public string Street { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string AddressRegion { get; init; } = string.Empty;
    public string AddressDetails { get; init; } = string.Empty;
    public SiteType SiteType { get; init; }
    public string? BSCName { get; init; }
    public string? BSCCode { get; init; }
    public string? Subcontractor { get; init; }
    public string? MaintenanceArea { get; init; }
}

