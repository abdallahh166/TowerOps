using TowerOps.Application.Common;
using TowerOps.Application.DTOs.ChecklistTemplates;
using TowerOps.Domain.Enums;

namespace TowerOps.Application.Queries.ChecklistTemplates.GetChecklistTemplateHistory;

public record GetChecklistTemplateHistoryQuery : IQuery<List<ChecklistTemplateDto>>
{
    public VisitType VisitType { get; init; }
}
