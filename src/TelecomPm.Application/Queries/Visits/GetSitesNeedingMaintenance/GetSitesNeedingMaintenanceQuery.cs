namespace TelecomPM.Application.Queries.Sites.GetSitesNeedingMaintenance;

using System;
using System.Collections.Generic;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Sites;

public record GetSitesNeedingMaintenanceQuery : IQuery<List<SiteDto>>
{
    public int DaysThreshold { get; init; } = 30;
    public Guid? OfficeId { get; init; }
}