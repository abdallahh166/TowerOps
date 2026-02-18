namespace TelecomPM.Application.Queries.Visits.GetVisitEvidenceStatus;

using MediatR;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Visits;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Interfaces.Repositories;

public sealed class GetVisitEvidenceStatusQueryHandler : IRequestHandler<GetVisitEvidenceStatusQuery, Result<VisitEvidenceStatusDto>>
{
    private const int RequiredPhotosPerType = 30;
    private const int RequiredReadingsCount = 15;

    private readonly IVisitRepository _visitRepository;

    public GetVisitEvidenceStatusQueryHandler(IVisitRepository visitRepository)
    {
        _visitRepository = visitRepository;
    }

    public async Task<Result<VisitEvidenceStatusDto>> Handle(GetVisitEvidenceStatusQuery request, CancellationToken cancellationToken)
    {
        var visit = await _visitRepository.GetByIdAsync(request.VisitId, cancellationToken);
        if (visit == null)
            return Result.Failure<VisitEvidenceStatusDto>("Visit not found");

        var beforePhotos = visit.Photos.Count(p => p.Type == PhotoType.Before);
        var afterPhotos = visit.Photos.Count(p => p.Type == PhotoType.After);
        var completedChecklistItems = visit.Checklists.Count(c => c.Status != CheckStatus.NA);

        var dto = new VisitEvidenceStatusDto
        {
            VisitId = visit.Id,
            BeforePhotos = beforePhotos,
            AfterPhotos = afterPhotos,
            RequiredBeforePhotos = RequiredPhotosPerType,
            RequiredAfterPhotos = RequiredPhotosPerType,
            ReadingsCount = visit.Readings.Count,
            RequiredReadings = RequiredReadingsCount,
            ChecklistItems = visit.Checklists.Count,
            CompletedChecklistItems = completedChecklistItems,
            CompletionPercentage = visit.CompletionPercentage,
            IsPhotosComplete = visit.IsPhotosComplete,
            IsReadingsComplete = visit.IsReadingsComplete,
            IsChecklistComplete = visit.IsChecklistComplete,
            CanBeSubmitted = visit.CanBeSubmitted()
        };

        return Result.Success(dto);
    }
}
