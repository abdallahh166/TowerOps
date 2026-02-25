using TowerOps.Application.Common;
using TowerOps.Application.DTOs.ChecklistTemplates;

namespace TowerOps.Application.Queries.ChecklistTemplates.GetChecklistTemplateById;

public record GetChecklistTemplateByIdQuery : IQuery<ChecklistTemplateDto>
{
    public Guid TemplateId { get; init; }
}
