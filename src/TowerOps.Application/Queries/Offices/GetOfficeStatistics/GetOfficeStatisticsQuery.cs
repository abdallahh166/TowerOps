namespace TowerOps.Application.Queries.Offices.GetOfficeStatistics;

using System;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Offices;

public record GetOfficeStatisticsQuery : IQuery<OfficeStatisticsDto>
{
    public Guid OfficeId { get; init; }
}

