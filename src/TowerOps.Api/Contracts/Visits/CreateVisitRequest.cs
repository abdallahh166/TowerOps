namespace TowerOps.Api.Contracts.Visits;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TowerOps.Domain.Enums;

public record CreateVisitRequest
{
    [Required]
    public Guid SiteId { get; init; }

    [Required]
    public Guid EngineerId { get; init; }

    [Required]
    public DateTime ScheduledDate { get; init; }

    public VisitType Type { get; init; } = VisitType.BM;

    public Guid? SupervisorId { get; init; }

    public List<string> TechnicianNames { get; init; } = new();
}

