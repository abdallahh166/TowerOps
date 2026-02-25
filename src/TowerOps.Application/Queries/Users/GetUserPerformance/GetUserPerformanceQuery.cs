namespace TowerOps.Application.Queries.Users.GetUserPerformance;

using System;
using System.Collections.Generic;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Users;

public record GetUserPerformanceQuery : IQuery<UserPerformanceDto>
{
    public Guid UserId { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
}

