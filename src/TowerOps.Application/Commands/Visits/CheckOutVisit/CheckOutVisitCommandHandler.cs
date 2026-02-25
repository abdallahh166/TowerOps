namespace TowerOps.Application.Commands.Visits.CheckOutVisit;

using MediatR;
using TowerOps.Application.Common;
using TowerOps.Application.Common.Interfaces;
using TowerOps.Domain.Exceptions;
using TowerOps.Domain.Interfaces.Repositories;
using TowerOps.Domain.ValueObjects;

public sealed class CheckOutVisitCommandHandler : IRequestHandler<CheckOutVisitCommand, Result>
{
    private readonly IVisitRepository _visitRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public CheckOutVisitCommandHandler(
        IVisitRepository visitRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _visitRepository = visitRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CheckOutVisitCommand request, CancellationToken cancellationToken)
    {
        var visit = await _visitRepository.GetByIdAsync(request.VisitId, cancellationToken);
        if (visit is null)
            return Result.Failure("Visit not found.");

        if (!_currentUserService.IsAuthenticated || _currentUserService.UserId == Guid.Empty)
            return Result.Failure("Authenticated user is required.");

        if (visit.EngineerId != _currentUserService.UserId)
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
