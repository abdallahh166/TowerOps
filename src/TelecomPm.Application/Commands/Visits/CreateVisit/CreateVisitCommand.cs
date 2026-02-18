namespace TelecomPM.Application.Commands.Visits.CreateVisit;

using System;
using System.Collections.Generic;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Visits;
using TelecomPM.Domain.Enums;

public record CreateVisitCommand : ICommand<VisitDto>
{
    public Guid SiteId { get; init; }
    public Guid EngineerId { get; init; }
    public DateTime ScheduledDate { get; init; }
    public VisitType Type { get; init; } = VisitType.PreventiveMaintenance;
    public Guid? SupervisorId { get; init; }
    public List<string> TechnicianNames { get; init; } = new();
}