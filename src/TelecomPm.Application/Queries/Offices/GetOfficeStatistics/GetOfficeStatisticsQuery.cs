namespace TelecomPM.Application.Queries.Offices.GetOfficeStatistics;

using System;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Offices;

public record GetOfficeStatisticsQuery : IQuery<OfficeStatisticsDto>
{
    public Guid OfficeId { get; init; }
}

