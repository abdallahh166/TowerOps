namespace TelecomPM.Application.Commands.Visits.AddReading;

using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Visits;
using TelecomPM.Domain.Entities.Visits;
using TelecomPM.Domain.Interfaces.Repositories;

public class AddReadingCommandHandler : IRequestHandler<AddReadingCommand, Result<VisitReadingDto>>
{
    private readonly IVisitRepository _visitRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AddReadingCommandHandler(
        IVisitRepository visitRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _visitRepository = visitRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<VisitReadingDto>> Handle(AddReadingCommand request, CancellationToken cancellationToken)
    {
        var visit = await _visitRepository.GetByIdAsync(request.VisitId, cancellationToken);
        if (visit == null)
            return Result.Failure<VisitReadingDto>("Visit not found");

        if (!visit.CanBeEdited())
            return Result.Failure<VisitReadingDto>("Visit cannot be edited");

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

        await _visitRepository.UpdateAsync(visit, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = _mapper.Map<VisitReadingDto>(reading);
        return Result.Success(dto);
    }
}