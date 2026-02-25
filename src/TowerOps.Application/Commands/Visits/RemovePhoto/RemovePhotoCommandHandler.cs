namespace TowerOps.Application.Commands.Visits.RemovePhoto;

using MediatR;
using System.Threading;
using System.Threading.Tasks;
using TowerOps.Application.Common;
using TowerOps.Domain.Interfaces.Repositories;

public class RemovePhotoCommandHandler : IRequestHandler<RemovePhotoCommand, Result>
{
    private readonly IVisitRepository _visitRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemovePhotoCommandHandler(
        IVisitRepository visitRepository,
        IUnitOfWork unitOfWork)
    {
        _visitRepository = visitRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(RemovePhotoCommand request, CancellationToken cancellationToken)
    {
        var visit = await _visitRepository.GetByIdAsync(request.VisitId, cancellationToken);
        if (visit == null)
            return Result.Failure("Visit not found");

        if (!visit.CanBeEdited())
            return Result.Failure("Visit cannot be edited");

        try
        {
            visit.RemovePhoto(request.PhotoId);
            await _visitRepository.UpdateAsync(visit, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (System.Exception ex)
        {
            return Result.Failure($"Failed to remove photo: {ex.Message}");
        }
    }
}

