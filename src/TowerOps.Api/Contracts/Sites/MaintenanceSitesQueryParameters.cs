namespace TowerOps.Api.Contracts.Sites;

using System;
using System.ComponentModel.DataAnnotations;

public class MaintenanceSitesQueryParameters
{
    [Range(1, 365)]
    public int DaysThreshold { get; init; } = 30;

    public Guid? OfficeId { get; init; }
}

