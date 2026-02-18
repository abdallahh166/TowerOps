namespace TelecomPM.Application.Commands.Visits.AddChecklistItem;

using AutoMapper;
using MediatR;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Visits;
using TelecomPM.Domain.Entities.Visits;
using TelecomPM.Domain.Interfaces.Repositories;

public class AddChecklistItemCommandHandler : IRequestHandler<AddChecklistItemCommand, Result<VisitChecklistDto>>
{
    private readonly IVisitRepository _visitRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AddChecklistItemCommandHandler(
        IVisitRepository visitRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _visitRepository = visitRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<VisitChecklistDto>> Handle(AddChecklistItemCommand request, CancellationToken cancellationToken)
    {
        var visit = await _visitRepository.GetByIdAsync(request.VisitId, cancellationToken);
        if (visit == null)
            return Result.Failure<VisitChecklistDto>("Visit not found");

        if (!visit.CanBeEdited())
            return Result.Failure<VisitChecklistDto>("Visit cannot be edited");

        try
        {
            var checklistItem = VisitChecklist.Create(
                visit.Id,
                request.Category,
                request.ItemName,
                request.Description,
                request.IsMandatory);

            visit.AddChecklistItem(checklistItem);

            await _visitRepository.UpdateAsync(visit, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var dto = _mapper.Map<VisitChecklistDto>(checklistItem);
            return Result.Success(dto);
        }
        catch (Exception ex)
        {
            return Result.Failure<VisitChecklistDto>($"Failed to add checklist item: {ex.Message}");
        }
    }
}
