namespace TowerOps.Api.Contracts.Visits;

using System;
using System.ComponentModel.DataAnnotations;
using TowerOps.Domain.Enums;

public class EngineerVisitQueryParameters
{
    private const int MaxPageSize = 100;

    private int pageSize = 10;

    [Range(1, int.MaxValue)]
    public int PageNumber { get; init; } = 1;

    [Range(1, MaxPageSize)]
    public int PageSize
    {
        get => pageSize;
        init => pageSize = Math.Clamp(value, 1, MaxPageSize);
    }

    public VisitStatus? Status { get; init; }

    public DateTime? From { get; init; }

    public DateTime? To { get; init; }
}

