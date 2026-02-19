namespace TelecomPM.Application.Services;

using TelecomPM.Application.Common;
using TelecomPM.Domain.Entities.Visits;
using TelecomPM.Domain.Interfaces.Repositories;

public sealed class EditableVisitMutationService : IEditableVisitMutationService
{
    private readonly IVisitRepository _visitRepository;
    private readonly IUnitOfWork _unitOfWork;

    public EditableVisitMutationService(IVisitRepository visitRepository, IUnitOfWork unitOfWork)
    {
        _visitRepository = visitRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<T>> ExecuteAsync<T>(
        Guid visitId,
        Func<Visit, Task<T>> mutation,
        string failurePrefix,
        CancellationToken cancellationToken)
    {
        var visit = await _visitRepository.GetByIdAsync(visitId, cancellationToken);
        if (visit == null)
        {
            return Result.Failure<T>("Visit not found");
        }

        if (!visit.CanBeEdited())
        {
            return Result.Failure<T>("Visit cannot be edited");
        }

        try
        {
            var response = await mutation(visit);
            await _visitRepository.UpdateAsync(visit, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success(response);
        }
        catch (Exception ex)
        {
            return Result.Failure<T>($"{failurePrefix}: {ex.Message}");
        }
    }
}
