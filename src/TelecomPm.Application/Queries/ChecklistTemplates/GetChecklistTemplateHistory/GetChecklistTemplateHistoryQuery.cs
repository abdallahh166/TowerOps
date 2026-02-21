using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.ChecklistTemplates;
using TelecomPM.Domain.Enums;

namespace TelecomPM.Application.Queries.ChecklistTemplates.GetChecklistTemplateHistory;

public record GetChecklistTemplateHistoryQuery : IQuery<List<ChecklistTemplateDto>>
{
    public VisitType VisitType { get; init; }
}
