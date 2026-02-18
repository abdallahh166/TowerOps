namespace TelecomPM.Application.Commands.Visits.AddChecklistItem;

using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Visits;

public record AddChecklistItemCommand : ICommand<VisitChecklistDto>
{
    public Guid VisitId { get; init; }
    public string Category { get; init; } = string.Empty;
    public string ItemName { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public bool IsMandatory { get; init; }
}
