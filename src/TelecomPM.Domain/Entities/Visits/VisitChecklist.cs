using TelecomPM.Domain.Common;
using TelecomPM.Domain.Enums;

namespace TelecomPM.Domain.Entities.Visits;

// ==================== Visit Checklist ====================
public sealed class VisitChecklist : Entity<Guid>
{
    public Guid VisitId { get; private set; }
    public string Category { get; private set; } = string.Empty; // Electrical, Power, Cooling
    public string ItemName { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public Guid? TemplateItemId { get; private set; }
    public CheckStatus Status { get; private set; }
    public bool IsMandatory { get; private set; }
    public string? Notes { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public List<Guid> RelatedPhotoIds { get; private set; } = new();

    private VisitChecklist() : base() { }

    private VisitChecklist(
        Guid visitId,
        string category,
        string itemName,
        string description,
        bool isMandatory,
        Guid? templateItemId) : base(Guid.NewGuid())
    {
        VisitId = visitId;
        Category = category;
        ItemName = itemName;
        Description = description;
        IsMandatory = isMandatory;
        TemplateItemId = templateItemId;
        Status = CheckStatus.NA;
    }

    public static VisitChecklist Create(
        Guid visitId,
        string category,
        string itemName,
        string description,
        bool isMandatory = false,
        Guid? templateItemId = null)
    {
        return new VisitChecklist(visitId, category, itemName, description, isMandatory, templateItemId);
    }

    public void UpdateStatus(CheckStatus status, string? notes = null)
    {
        Status = status;
        Notes = notes;
        
        if (status != CheckStatus.NA)
        {
            CompletedAt = DateTime.UtcNow;
        }
    }

    public void AttachPhoto(Guid photoId)
    {
        if (!RelatedPhotoIds.Contains(photoId))
        {
            RelatedPhotoIds.Add(photoId);
        }
    }
}
