using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.ChecklistTemplates;

namespace TelecomPM.Application.Queries.ChecklistTemplates.GetChecklistTemplateById;

public record GetChecklistTemplateByIdQuery : IQuery<ChecklistTemplateDto>
{
    public Guid TemplateId { get; init; }
}
