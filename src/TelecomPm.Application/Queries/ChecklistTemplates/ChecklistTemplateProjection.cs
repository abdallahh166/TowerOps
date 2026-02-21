using TelecomPM.Application.DTOs.ChecklistTemplates;
using TelecomPM.Domain.Entities.ChecklistTemplates;

namespace TelecomPM.Application.Queries.ChecklistTemplates;

internal static class ChecklistTemplateProjection
{
    public static ChecklistTemplateDto ToDto(ChecklistTemplate template)
    {
        return new ChecklistTemplateDto
        {
            Id = template.Id,
            VisitType = template.VisitType,
            Version = template.Version,
            IsActive = template.IsActive,
            EffectiveFromUtc = template.EffectiveFromUtc,
            EffectiveToUtc = template.EffectiveToUtc,
            CreatedBy = template.CreatedBy,
            ApprovedBy = template.ApprovedBy,
            ApprovedAtUtc = template.ApprovedAtUtc,
            ChangeNotes = template.ChangeNotes,
            Items = template.Items
                .OrderBy(i => i.OrderIndex)
                .Select(i => new ChecklistTemplateItemDto
                {
                    Id = i.Id,
                    ChecklistTemplateId = i.ChecklistTemplateId,
                    Category = i.Category,
                    ItemName = i.ItemName,
                    Description = i.Description,
                    IsMandatory = i.IsMandatory,
                    OrderIndex = i.OrderIndex,
                    ApplicableSiteTypes = i.ApplicableSiteTypes,
                    ApplicableVisitTypes = i.ApplicableVisitTypes
                })
                .ToList()
        };
    }
}
