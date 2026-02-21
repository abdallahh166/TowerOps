using TelecomPM.Application.Common;
using TelecomPM.Domain.Enums;

namespace TelecomPM.Application.Commands.ChecklistTemplates.CreateChecklistTemplate;

public sealed record CreateChecklistTemplateItemModel
{
    public string Category { get; init; } = string.Empty;
    public string ItemName { get; init; } = string.Empty;
    public string? Description { get; init; }
    public bool IsMandatory { get; init; }
    public int OrderIndex { get; init; }
    public string? ApplicableSiteTypes { get; init; }
    public string? ApplicableVisitTypes { get; init; }
}

public record CreateChecklistTemplateCommand : ICommand<Guid>
{
    public VisitType VisitType { get; init; }
    public string Version { get; init; } = string.Empty;
    public DateTime EffectiveFromUtc { get; init; }
    public string? ChangeNotes { get; init; }
    public string CreatedBy { get; init; } = string.Empty;
    public List<CreateChecklistTemplateItemModel> Items { get; init; } = new();
}
