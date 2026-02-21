namespace TelecomPM.Application.DTOs.ChecklistTemplates;

public sealed record ChecklistTemplateItemDto
{
    public Guid Id { get; init; }
    public Guid ChecklistTemplateId { get; init; }
    public string Category { get; init; } = string.Empty;
    public string ItemName { get; init; } = string.Empty;
    public string? Description { get; init; }
    public bool IsMandatory { get; init; }
    public int OrderIndex { get; init; }
    public string? ApplicableSiteTypes { get; init; }
    public string? ApplicableVisitTypes { get; init; }
}
