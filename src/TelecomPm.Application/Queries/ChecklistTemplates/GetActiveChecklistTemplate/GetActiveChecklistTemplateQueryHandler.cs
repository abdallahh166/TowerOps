using MediatR;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.ChecklistTemplates;
using TelecomPM.Domain.Interfaces.Repositories;

namespace TelecomPM.Application.Queries.ChecklistTemplates.GetActiveChecklistTemplate;

public class GetActiveChecklistTemplateQueryHandler : IRequestHandler<GetActiveChecklistTemplateQuery, Result<ChecklistTemplateDto>>
{
    private readonly IChecklistTemplateRepository _checklistTemplateRepository;

    public GetActiveChecklistTemplateQueryHandler(IChecklistTemplateRepository checklistTemplateRepository)
    {
        _checklistTemplateRepository = checklistTemplateRepository;
    }

    public async Task<Result<ChecklistTemplateDto>> Handle(GetActiveChecklistTemplateQuery request, CancellationToken cancellationToken)
    {
        var template = await _checklistTemplateRepository.GetActiveByVisitTypeAsync(request.VisitType, cancellationToken);
        if (template is null)
            return Result.Failure<ChecklistTemplateDto>("Checklist template not found");

        return Result.Success(ChecklistTemplateProjection.ToDto(template));
    }
}
