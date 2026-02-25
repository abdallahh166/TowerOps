namespace TowerOps.Application.Commands.Visits.CreateVisit;

using System;
using System.Collections.Generic;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Visits;
using TowerOps.Domain.Enums;

public record CreateVisitCommand : ICommand<VisitDto>
{
    public Guid SiteId { get; init; }
    public Guid EngineerId { get; init; }
    public DateTime ScheduledDate { get; init; }
    public VisitType Type { get; init; } = VisitType.BM;
    public Guid? SupervisorId { get; init; }
    public List<string> TechnicianNames { get; init; } = new();
}
