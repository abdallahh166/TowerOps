using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.ChecklistTemplates;
using TelecomPM.Domain.Enums;

namespace TelecomPM.Application.Queries.ChecklistTemplates.GetActiveChecklistTemplate;

public record GetActiveChecklistTemplateQuery : IQuery<ChecklistTemplateDto>
{
    public VisitType VisitType { get; init; }
}
