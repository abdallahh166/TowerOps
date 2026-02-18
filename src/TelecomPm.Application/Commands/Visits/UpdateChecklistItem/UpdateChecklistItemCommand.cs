namespace TelecomPM.Application.Commands.Visits.UpdateChecklistItem;

using System;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Visits;
using TelecomPM.Domain.Enums;

public record UpdateChecklistItemCommand : ICommand<VisitChecklistDto>
{
    public Guid VisitId { get; init; }
    public Guid ChecklistItemId { get; init; }
    public CheckStatus Status { get; init; }
    public string? Notes { get; init; }
}

