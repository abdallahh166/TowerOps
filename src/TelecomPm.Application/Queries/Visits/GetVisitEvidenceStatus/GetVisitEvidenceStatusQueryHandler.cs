namespace TelecomPM.Application.Queries.Visits.GetVisitEvidenceStatus;

using MediatR;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Visits;
using TelecomPM.Domain.Services;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Interfaces.Repositories;
using TelecomPM.Domain.ValueObjects;

public sealed class GetVisitEvidenceStatusQueryHandler : IRequestHandler<GetVisitEvidenceStatusQuery, Result<VisitEvidenceStatusDto>>
{
    private readonly IVisitRepository _visitRepository;
    private readonly IEvidencePolicyService _evidencePolicyService;

    public GetVisitEvidenceStatusQueryHandler(
        IVisitRepository visitRepository,
        IEvidencePolicyService evidencePolicyService)
    {
        _visitRepository = visitRepository;
        _evidencePolicyService = evidencePolicyService;
    }

    public async Task<Result<VisitEvidenceStatusDto>> Handle(GetVisitEvidenceStatusQuery request, CancellationToken cancellationToken)
    {
        var visit = await _visitRepository.GetByIdAsync(request.VisitId, cancellationToken);
        if (visit == null)
            return Result.Failure<VisitEvidenceStatusDto>("Visit not found");

        var beforePhotos = visit.Photos.Count(p => p.Type == PhotoType.Before);
        var afterPhotos = visit.Photos.Count(p => p.Type == PhotoType.After);
        var completedChecklistItems = visit.Checklists.Count(c => c.Status != CheckStatus.NA);
        var effectivePolicy = _evidencePolicyService.GetEffectivePolicy(visit.Type, EvidencePolicy.DefaultFor(visit.Type));

        var requiredPhotos = effectivePolicy.MinPhotosRequired;
        var requiredReadings = effectivePolicy.ReadingsRequired ? 1 : 0;
        var isReadingsComplete = !effectivePolicy.ReadingsRequired || visit.Readings.Any();
        var checklistCompletionPercent = visit.Checklists.Count == 0
            ? 0
            : completedChecklistItems * 100 / visit.Checklists.Count;
        var isChecklistComplete = !effectivePolicy.ChecklistRequired ||
                                  checklistCompletionPercent >= effectivePolicy.MinChecklistCompletionPercent;
        var isPhotosComplete = visit.Photos.Count >= requiredPhotos;
        var canBeSubmitted =
            (visit.Status == VisitStatus.Completed || visit.Status == VisitStatus.NeedsCorrection) &&
            isPhotosComplete &&
            isReadingsComplete &&
            isChecklistComplete;

        var dto = new VisitEvidenceStatusDto
        {
            VisitId = visit.Id,
            BeforePhotos = beforePhotos,
            AfterPhotos = afterPhotos,
            RequiredBeforePhotos = requiredPhotos,
            RequiredAfterPhotos = requiredPhotos,
            ReadingsCount = visit.Readings.Count,
            RequiredReadings = requiredReadings,
            ChecklistItems = visit.Checklists.Count,
            CompletedChecklistItems = completedChecklistItems,
            CompletionPercentage = visit.CompletionPercentage,
            IsPhotosComplete = isPhotosComplete,
            IsReadingsComplete = isReadingsComplete,
            IsChecklistComplete = isChecklistComplete,
            CanBeSubmitted = canBeSubmitted
        };

        return Result.Success(dto);
    }
}
