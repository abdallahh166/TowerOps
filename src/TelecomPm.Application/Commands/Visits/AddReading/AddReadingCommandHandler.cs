namespace TelecomPM.Application.Commands.Visits.AddReading;

using AutoMapper;
using MediatR;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Visits;
using TelecomPM.Application.Services;
using TelecomPM.Domain.Entities.Visits;

public class AddReadingCommandHandler : IRequestHandler<AddReadingCommand, Result<VisitReadingDto>>
{
    private readonly IEditableVisitMutationService _editableVisitMutationService;
    private readonly IMapper _mapper;

    public AddReadingCommandHandler(IEditableVisitMutationService editableVisitMutationService, IMapper mapper)
    {
        _editableVisitMutationService = editableVisitMutationService;
        _mapper = mapper;
    }

    public Task<Result<VisitReadingDto>> Handle(AddReadingCommand request, CancellationToken cancellationToken)
        => _editableVisitMutationService.ExecuteAsync(
            request.VisitId,
            visit =>
            {
                var reading = VisitReading.Create(
                    visit.Id,
                    request.ReadingType,
                    request.Category,
                    request.Value,
                    request.Unit);

                if (request.MinAcceptable.HasValue && request.MaxAcceptable.HasValue)
                {
                    reading.SetValidationRange(request.MinAcceptable.Value, request.MaxAcceptable.Value);
                }

                if (!string.IsNullOrWhiteSpace(request.Phase))
                {
                    reading.SetPhase(request.Phase);
                }

                if (!string.IsNullOrWhiteSpace(request.Equipment))
                {
                    reading.SetEquipment(request.Equipment);
                }

                if (!string.IsNullOrWhiteSpace(request.Notes))
                {
                    reading.AddNotes(request.Notes);
                }

                visit.AddReading(reading);

                var dto = _mapper.Map<VisitReadingDto>(reading);
                return Task.FromResult(dto);
            },
            "Failed to add reading",
            cancellationToken);
}
