namespace TowerOps.Application.Services;

using TowerOps.Application.Common;
using TowerOps.Domain.Entities.Visits;

public interface IEditableVisitMutationService
{
    Task<Result<T>> ExecuteAsync<T>(
        Guid visitId,
        Func<Visit, Task<T>> mutation,
        string failurePrefix,
        CancellationToken cancellationToken);
}
