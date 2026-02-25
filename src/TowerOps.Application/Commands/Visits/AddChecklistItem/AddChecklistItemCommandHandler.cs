namespace TowerOps.Application.Commands.Visits.AddChecklistItem;

using AutoMapper;
using MediatR;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Visits;
using TowerOps.Application.Services;
using TowerOps.Domain.Entities.Visits;

public class AddChecklistItemCommandHandler : IRequestHandler<AddChecklistItemCommand, Result<VisitChecklistDto>>
{
    private readonly IEditableVisitMutationService _editableVisitMutationService;
    private readonly IMapper _mapper;

    public AddChecklistItemCommandHandler(IEditableVisitMutationService editableVisitMutationService, IMapper mapper)
    {
        _editableVisitMutationService = editableVisitMutationService;
        _mapper = mapper;
    }

    public Task<Result<VisitChecklistDto>> Handle(AddChecklistItemCommand request, CancellationToken cancellationToken)
        => _editableVisitMutationService.ExecuteAsync(
            request.VisitId,
            visit =>
            {
                var checklistItem = VisitChecklist.Create(
                    visit.Id,
                    request.Category,
                    request.ItemName,
                    request.Description,
                    request.IsMandatory);

                visit.AddChecklistItem(checklistItem);

                var dto = _mapper.Map<VisitChecklistDto>(checklistItem);
                return Task.FromResult(dto);
            },
            "Failed to add checklist item",
            cancellationToken);
}
