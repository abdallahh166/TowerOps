namespace TelecomPM.Application.Commands.Visits.CheckOutVisit;

using MediatR;
using TelecomPM.Application.Common;
using TelecomPM.Domain.Exceptions;
using TelecomPM.Domain.Interfaces.Repositories;
using TelecomPM.Domain.ValueObjects;

public sealed class CheckOutVisitCommandHandler : IRequestHandler<CheckOutVisitCommand, Result>
{
    private readonly IVisitRepository _visitRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CheckOutVisitCommandHandler(
        IVisitRepository visitRepository,
        IUnitOfWork unitOfWork)
    {
        _visitRepository = visitRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CheckOutVisitCommand request, CancellationToken cancellationToken)
    {
        var visit = await _visitRepository.GetByIdAsync(request.VisitId, cancellationToken);
        if (visit is null)
            return Result.Failure("Visit not found.");

        if (visit.EngineerId != request.EngineerId)
            return Result.Failure("Only the assigned engineer can check out.");

        try
        {
            var geoLocation = GeoLocation.Create(request.Latitude, request.Longitude);
            visit.RecordCheckOut(geoLocation);

            await _visitRepository.UpdateAsync(visit, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message);
        }
    }
}
