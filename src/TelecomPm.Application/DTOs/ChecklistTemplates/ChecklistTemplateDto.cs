using TelecomPM.Domain.Enums;

namespace TelecomPM.Application.DTOs.ChecklistTemplates;

public sealed record ChecklistTemplateDto
{
    public Guid Id { get; init; }
    public VisitType VisitType { get; init; }
    public string Version { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public DateTime EffectiveFromUtc { get; init; }
    public DateTime? EffectiveToUtc { get; init; }
    public string CreatedBy { get; init; } = string.Empty;
    public string? ApprovedBy { get; init; }
    public DateTime? ApprovedAtUtc { get; init; }
    public string? ChangeNotes { get; init; }
    public List<ChecklistTemplateItemDto> Items { get; init; } = new();
}
