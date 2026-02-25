using TowerOps.Api.Contracts.ChecklistTemplates;
using TowerOps.Application.Commands.ChecklistTemplates.ActivateChecklistTemplate;
using TowerOps.Application.Commands.ChecklistTemplates.CreateChecklistTemplate;
using TowerOps.Application.Commands.Imports.ImportChecklistTemplate;
using TowerOps.Application.Queries.ChecklistTemplates.GetActiveChecklistTemplate;
using TowerOps.Application.Queries.ChecklistTemplates.GetChecklistTemplateById;
using TowerOps.Application.Queries.ChecklistTemplates.GetChecklistTemplateHistory;
using TowerOps.Domain.Enums;

namespace TowerOps.Api.Mappings;

public static class ChecklistTemplatesContractMapper
{
    public static GetActiveChecklistTemplateQuery ToGetActiveQuery(this VisitType visitType)
        => new() { VisitType = visitType };

    public static GetChecklistTemplateByIdQuery ToGetByIdQuery(this Guid templateId)
        => new() { TemplateId = templateId };

    public static GetChecklistTemplateHistoryQuery ToGetHistoryQuery(this VisitType visitType)
        => new() { VisitType = visitType };

    public static CreateChecklistTemplateCommand ToCommand(this CreateChecklistTemplateRequest request)
        => new()
        {
            VisitType = request.VisitType,
            Version = request.Version,
            EffectiveFromUtc = request.EffectiveFromUtc,
            ChangeNotes = request.ChangeNotes,
            CreatedBy = request.CreatedBy,
            Items = request.Items.Select(i => new CreateChecklistTemplateItemModel
            {
                Category = i.Category,
                ItemName = i.ItemName,
                Description = i.Description,
                IsMandatory = i.IsMandatory,
                OrderIndex = i.OrderIndex,
                ApplicableSiteTypes = i.ApplicableSiteTypes,
                ApplicableVisitTypes = i.ApplicableVisitTypes
            }).ToList()
        };

    public static ActivateChecklistTemplateCommand ToCommand(this ActivateChecklistTemplateRequest request, Guid templateId)
        => new()
        {
            TemplateId = templateId,
            ApprovedBy = request.ApprovedBy
        };

    public static ImportChecklistTemplateCommand ToImportCommand(this ImportChecklistTemplateRequest request, byte[] fileContent)
        => new()
        {
            FileContent = fileContent,
            VisitType = request.VisitType,
            Version = request.Version,
            EffectiveFromUtc = request.EffectiveFromUtc,
            CreatedBy = request.CreatedBy,
            ChangeNotes = request.ChangeNotes
        };
}
