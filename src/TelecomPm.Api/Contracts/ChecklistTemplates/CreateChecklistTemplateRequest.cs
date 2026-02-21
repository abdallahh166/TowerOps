using TelecomPM.Domain.Enums;

namespace TelecomPm.Api.Contracts.ChecklistTemplates;

public sealed class CreateChecklistTemplateRequest
{
    public VisitType VisitType { get; set; }
    public string Version { get; set; } = string.Empty;
    public DateTime EffectiveFromUtc { get; set; }
    public string? ChangeNotes { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public List<CreateChecklistTemplateItemRequest> Items { get; set; } = new();
}

public sealed class CreateChecklistTemplateItemRequest
{
    public string Category { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsMandatory { get; set; }
    public int OrderIndex { get; set; }
    public string? ApplicableSiteTypes { get; set; }
    public string? ApplicableVisitTypes { get; set; }
}
