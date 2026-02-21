using TelecomPM.Domain.Exceptions;

namespace TelecomPM.Domain.Entities.ChecklistTemplates;

public sealed class ChecklistTemplateItem
{
    public Guid Id { get; private set; }
    public Guid ChecklistTemplateId { get; private set; }
    public string Category { get; private set; } = string.Empty;
    public string ItemName { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public bool IsMandatory { get; private set; }
    public int OrderIndex { get; private set; }
    public string? ApplicableSiteTypes { get; private set; }
    public string? ApplicableVisitTypes { get; private set; }

    private ChecklistTemplateItem()
    {
    }

    private ChecklistTemplateItem(
        Guid checklistTemplateId,
        string category,
        string itemName,
        string? description,
        bool isMandatory,
        int orderIndex,
        string? applicableSiteTypes,
        string? applicableVisitTypes)
    {
        Id = Guid.NewGuid();
        ChecklistTemplateId = checklistTemplateId;
        Category = category.Trim();
        ItemName = itemName.Trim();
        Description = description?.Trim();
        IsMandatory = isMandatory;
        OrderIndex = orderIndex;
        ApplicableSiteTypes = applicableSiteTypes;
        ApplicableVisitTypes = applicableVisitTypes;
    }

    public static ChecklistTemplateItem Create(
        Guid checklistTemplateId,
        string category,
        string itemName,
        string? description,
        bool isMandatory,
        int orderIndex,
        string? applicableSiteTypes,
        string? applicableVisitTypes = null)
    {
        if (checklistTemplateId == Guid.Empty)
            throw new DomainException("Checklist template ID is required");

        if (string.IsNullOrWhiteSpace(category))
            throw new DomainException("Checklist item category is required");

        if (string.IsNullOrWhiteSpace(itemName))
            throw new DomainException("Checklist item name is required");

        return new ChecklistTemplateItem(
            checklistTemplateId,
            category,
            itemName,
            description,
            isMandatory,
            orderIndex,
            applicableSiteTypes,
            applicableVisitTypes);
    }
}
