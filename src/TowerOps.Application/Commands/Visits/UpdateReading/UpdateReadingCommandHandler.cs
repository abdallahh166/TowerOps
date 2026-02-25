namespace TowerOps.Application.Commands.Visits.UpdateReading;

using AutoMapper;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Visits;
using TowerOps.Domain.Interfaces.Repositories;

public class UpdateReadingCommandHandler : IRequestHandler<UpdateReadingCommand, Result<VisitReadingDto>>
{
    private readonly IVisitRepository _visitRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateReadingCommandHandler(
        IVisitRepository visitRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _visitRepository = visitRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<VisitReadingDto>> Handle(UpdateReadingCommand request, CancellationToken cancellationToken)
    {
        var visit = await _visitRepository.GetByIdAsync(request.VisitId, cancellationToken);
        if (visit == null)
            return Result.Failure<VisitReadingDto>("Visit not found");

        if (!visit.CanBeEdited())
            return Result.Failure<VisitReadingDto>("Visit cannot be edited");

        try
        {
            visit.UpdateReading(request.ReadingId, request.Value);

            await _visitRepository.UpdateAsync(visit, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var reading = visit.Readings.FirstOrDefault(r => r.Id == request.ReadingId);
            if (reading == null)
                return Result.Failure<VisitReadingDto>("Reading not found");

            var dto = _mapper.Map<VisitReadingDto>(reading);
            return Result.Success(dto);
        }
        catch (System.Exception ex)
        {
            return Result.Failure<VisitReadingDto>($"Failed to update reading: {ex.Message}");
        }
    }
}

