using MediatR;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.ChecklistTemplates;
using TelecomPM.Domain.Interfaces.Repositories;

namespace TelecomPM.Application.Queries.ChecklistTemplates.GetChecklistTemplateHistory;

public class GetChecklistTemplateHistoryQueryHandler : IRequestHandler<GetChecklistTemplateHistoryQuery, Result<List<ChecklistTemplateDto>>>
{
    private readonly IChecklistTemplateRepository _checklistTemplateRepository;

    public GetChecklistTemplateHistoryQueryHandler(IChecklistTemplateRepository checklistTemplateRepository)
    {
        _checklistTemplateRepository = checklistTemplateRepository;
    }

    public async Task<Result<List<ChecklistTemplateDto>>> Handle(GetChecklistTemplateHistoryQuery request, CancellationToken cancellationToken)
    {
        var templates = await _checklistTemplateRepository.GetByVisitTypeAsync(request.VisitType, cancellationToken);
        var result = templates
            .OrderByDescending(t => t.EffectiveFromUtc)
            .Select(ChecklistTemplateProjection.ToDto)
            .ToList();

        return Result.Success(result);
    }
}
