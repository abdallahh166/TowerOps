using TowerOps.Application.Common;
using TowerOps.Application.DTOs.ChecklistTemplates;
using TowerOps.Domain.Enums;

namespace TowerOps.Application.Queries.ChecklistTemplates.GetActiveChecklistTemplate;

public record GetActiveChecklistTemplateQuery : IQuery<ChecklistTemplateDto>
{
    public VisitType VisitType { get; init; }
}
