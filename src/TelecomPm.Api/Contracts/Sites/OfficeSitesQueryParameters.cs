namespace TelecomPm.Api.Contracts.Sites;

using System;
using System.ComponentModel.DataAnnotations;
using TelecomPM.Domain.Enums;

public class OfficeSitesQueryParameters
{
    private const int MaxPageSize = 100;
    private int pageSize = 20;

    [Range(1, int.MaxValue)]
    public int PageNumber { get; init; } = 1;

    [Range(1, MaxPageSize)]
    public int PageSize
    {
        get => pageSize;
        init => pageSize = Math.Clamp(value, 1, MaxPageSize);
    }

    public SiteComplexity? Complexity { get; init; }

    public SiteStatus? Status { get; init; }
}

