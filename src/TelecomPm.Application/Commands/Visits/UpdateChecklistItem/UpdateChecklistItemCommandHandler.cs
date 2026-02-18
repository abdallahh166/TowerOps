namespace TelecomPM.Application.Commands.Visits.UpdateChecklistItem;

using AutoMapper;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Visits;
using TelecomPM.Domain.Interfaces.Repositories;

public class UpdateChecklistItemCommandHandler : IRequestHandler<UpdateChecklistItemCommand, Result<VisitChecklistDto>>
{
    private readonly IVisitRepository _visitRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateChecklistItemCommandHandler(
        IVisitRepository visitRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _visitRepository = visitRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<VisitChecklistDto>> Handle(UpdateChecklistItemCommand request, CancellationToken cancellationToken)
    {
        var visit = await _visitRepository.GetByIdAsync(request.VisitId, cancellationToken);
        if (visit == null)
            return Result.Failure<VisitChecklistDto>("Visit not found");

        if (!visit.CanBeEdited())
            return Result.Failure<VisitChecklistDto>("Visit cannot be edited");

        try
        {
            visit.UpdateChecklistItem(request.ChecklistItemId, request.Status, request.Notes);

            await _visitRepository.UpdateAsync(visit, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var checklistItem = visit.Checklists.FirstOrDefault(c => c.Id == request.ChecklistItemId);
            if (checklistItem == null)
                return Result.Failure<VisitChecklistDto>("Checklist item not found");

            var dto = _mapper.Map<VisitChecklistDto>(checklistItem);
            return Result.Success(dto);
        }
        catch (System.Exception ex)
        {
            return Result.Failure<VisitChecklistDto>($"Failed to update checklist item: {ex.Message}");
        }
    }
}

