using TowerOps.Application.Common;

namespace TowerOps.Application.Commands.ChecklistTemplates.ActivateChecklistTemplate;

public record ActivateChecklistTemplateCommand : ICommand
{
    public Guid TemplateId { get; init; }
    public string ApprovedBy { get; init; } = string.Empty;
}
