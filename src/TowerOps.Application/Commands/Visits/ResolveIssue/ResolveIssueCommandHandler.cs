namespace TowerOps.Application.Commands.Visits.ResolveIssue;

using MediatR;
using System.Threading;
using System.Threading.Tasks;
using TowerOps.Application.Common;
using TowerOps.Domain.Interfaces.Repositories;

public class ResolveIssueCommandHandler : IRequestHandler<ResolveIssueCommand, Result>
{
    private readonly IVisitRepository _visitRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ResolveIssueCommandHandler(
        IVisitRepository visitRepository,
        IUnitOfWork unitOfWork)
    {
        _visitRepository = visitRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ResolveIssueCommand request, CancellationToken cancellationToken)
    {
        var visit = await _visitRepository.GetByIdAsync(request.VisitId, cancellationToken);
        if (visit == null)
            return Result.Failure("Visit not found");

        try
        {
            visit.ResolveIssue(request.IssueId, request.Resolution);
            await _visitRepository.UpdateAsync(visit, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (System.Exception ex)
        {
            return Result.Failure($"Failed to resolve issue: {ex.Message}");
        }
    }
}

