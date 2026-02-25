namespace TowerOps.Application.Commands.Offices.UpdateOfficeStatistics;

using System;
using TowerOps.Application.Common;

public record UpdateOfficeStatisticsCommand : ICommand
{
    public Guid OfficeId { get; init; }
    public int TotalSites { get; init; }
    public int ActiveEngineers { get; init; }
    public int ActiveTechnicians { get; init; }
}

