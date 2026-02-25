namespace TowerOps.Application.Commands.Visits.UpdateChecklistItem;

using System;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Visits;
using TowerOps.Domain.Enums;

public record UpdateChecklistItemCommand : ICommand<VisitChecklistDto>
{
    public Guid VisitId { get; init; }
    public Guid ChecklistItemId { get; init; }
    public CheckStatus Status { get; init; }
    public string? Notes { get; init; }
}

