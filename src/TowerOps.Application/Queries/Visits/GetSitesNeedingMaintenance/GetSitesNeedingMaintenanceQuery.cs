namespace TowerOps.Application.Queries.Sites.GetSitesNeedingMaintenance;

using System;
using System.Collections.Generic;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Sites;

public record GetSitesNeedingMaintenanceQuery : IQuery<List<SiteDto>>
{
    public int DaysThreshold { get; init; } = 30;
    public Guid? OfficeId { get; init; }
}