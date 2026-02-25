using Microsoft.AspNetCore.Http;
using TowerOps.Domain.Enums;

namespace TowerOps.Api.Contracts.ChecklistTemplates;

public sealed class ImportChecklistTemplateRequest
{
    public IFormFile File { get; set; } = null!;
    public VisitType VisitType { get; set; }
    public string Version { get; set; } = "v1.0";
    public DateTime EffectiveFromUtc { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = string.Empty;
    public string? ChangeNotes { get; set; }
}
