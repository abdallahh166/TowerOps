using MediatR;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.ChecklistTemplates;
using TowerOps.Domain.Interfaces.Repositories;

namespace TowerOps.Application.Queries.ChecklistTemplates.GetChecklistTemplateById;

public class GetChecklistTemplateByIdQueryHandler : IRequestHandler<GetChecklistTemplateByIdQuery, Result<ChecklistTemplateDto>>
{
    private readonly IChecklistTemplateRepository _checklistTemplateRepository;

    public GetChecklistTemplateByIdQueryHandler(IChecklistTemplateRepository checklistTemplateRepository)
    {
        _checklistTemplateRepository = checklistTemplateRepository;
    }

    public async Task<Result<ChecklistTemplateDto>> Handle(GetChecklistTemplateByIdQuery request, CancellationToken cancellationToken)
    {
        var template = await _checklistTemplateRepository.GetByIdAsNoTrackingAsync(request.TemplateId, cancellationToken);
        if (template is null)
            return Result.Failure<ChecklistTemplateDto>("Checklist template not found");

        return Result.Success(ChecklistTemplateProjection.ToDto(template));
    }
}
