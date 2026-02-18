namespace TelecomPM.Application.Commands.Visits.CancelVisit;

using MediatR;
using System.Threading;
using System.Threading.Tasks;
using TelecomPM.Application.Common;
using TelecomPM.Domain.Interfaces.Repositories;

public class CancelVisitCommandHandler : IRequestHandler<CancelVisitCommand, Result>
{
    private readonly IVisitRepository _visitRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CancelVisitCommandHandler(
        IVisitRepository visitRepository,
        IUnitOfWork unitOfWork)
    {
        _visitRepository = visitRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CancelVisitCommand request, CancellationToken cancellationToken)
    {
        var visit = await _visitRepository.GetByIdAsync(request.VisitId, cancellationToken);
        if (visit == null)
            return Result.Failure("Visit not found");

        try
        {
            visit.Cancel(request.Reason);
            await _visitRepository.UpdateAsync(visit, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (System.Exception ex)
        {
            return Result.Failure($"Failed to cancel visit: {ex.Message}");
        }
    }
}

