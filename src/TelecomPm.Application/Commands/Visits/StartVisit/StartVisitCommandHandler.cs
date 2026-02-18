namespace TelecomPM.Application.Commands.Visits.StartVisit;

using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Visits;
using TelecomPM.Domain.Exceptions;
using TelecomPM.Domain.Interfaces.Repositories;
using TelecomPM.Domain.ValueObjects;

public class StartVisitCommandHandler : IRequestHandler<StartVisitCommand, Result<VisitDto>>
{
    private readonly IVisitRepository _visitRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public StartVisitCommandHandler(
        IVisitRepository visitRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _visitRepository = visitRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<VisitDto>> Handle(StartVisitCommand request, CancellationToken cancellationToken)
    {
        var visit = await _visitRepository.GetByIdAsync(request.VisitId, cancellationToken);
        if (visit == null)
            return Result.Failure<VisitDto>("Visit not found");

        try
        {
            var coordinates = Coordinates.Create(request.Latitude, request.Longitude);
            visit.StartVisit(coordinates);

            await _visitRepository.UpdateAsync(visit, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var dto = _mapper.Map<VisitDto>(visit);
            return Result.Success(dto);
        }
        catch (DomainException ex)
        {
            return Result.Failure<VisitDto>(ex.Message);
        }
    }
}