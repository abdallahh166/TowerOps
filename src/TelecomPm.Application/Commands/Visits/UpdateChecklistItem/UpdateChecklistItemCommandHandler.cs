namespace TelecomPM.Application.Commands.Visits.UpdateChecklistItem;

using AutoMapper;
using MediatR;
using System.Linq;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Visits;
using TelecomPM.Application.Services;

public class UpdateChecklistItemCommandHandler : IRequestHandler<UpdateChecklistItemCommand, Result<VisitChecklistDto>>
{
    private readonly IEditableVisitMutationService _editableVisitMutationService;
    private readonly IMapper _mapper;

    public UpdateChecklistItemCommandHandler(IEditableVisitMutationService editableVisitMutationService, IMapper mapper)
    {
        _editableVisitMutationService = editableVisitMutationService;
        _mapper = mapper;
    }

    public Task<Result<VisitChecklistDto>> Handle(UpdateChecklistItemCommand request, CancellationToken cancellationToken)
        => _editableVisitMutationService.ExecuteAsync(
            request.VisitId,
            visit =>
            {
                visit.UpdateChecklistItem(request.ChecklistItemId, request.Status, request.Notes);

                var checklistItem = visit.Checklists.FirstOrDefault(c => c.Id == request.ChecklistItemId)
                    ?? throw new InvalidOperationException("Checklist item not found");

                var dto = _mapper.Map<VisitChecklistDto>(checklistItem);
                return Task.FromResult(dto);
            },
            "Failed to update checklist item",
            cancellationToken);
}
