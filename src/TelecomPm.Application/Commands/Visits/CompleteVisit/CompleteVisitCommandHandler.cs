namespace TelecomPM.Application.Commands.Visits.CompleteVisit;

using MediatR;
using System.Threading;
using System.Threading.Tasks;
using TelecomPM.Application.Common;
using TelecomPM.Domain.Exceptions;
using TelecomPM.Domain.Interfaces.Repositories;

public class CompleteVisitCommandHandler : IRequestHandler<CompleteVisitCommand, Result>
{
    private readonly IVisitRepository _visitRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CompleteVisitCommandHandler(
        IVisitRepository visitRepository,
        IUnitOfWork unitOfWork)
    {
        _visitRepository = visitRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CompleteVisitCommand request, CancellationToken cancellationToken)
    {
        var visit = await _visitRepository.GetByIdAsync(request.VisitId, cancellationToken);
        if (visit == null)
            return Result.Failure("Visit not found");

        try
        {
            if (!string.IsNullOrWhiteSpace(request.EngineerNotes))
            {
                visit.AddEngineerNotes(request.EngineerNotes);
            }

            visit.CompleteVisit();

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