using MediatR;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.ChecklistTemplates;
using TelecomPM.Domain.Interfaces.Repositories;

namespace TelecomPM.Application.Queries.ChecklistTemplates.GetChecklistTemplateById;

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
