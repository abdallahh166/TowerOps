namespace TelecomPM.Application.Services;

using TelecomPM.Application.Common;
using TelecomPM.Domain.Entities.Visits;

public interface IEditableVisitMutationService
{
    Task<Result<T>> ExecuteAsync<T>(
        Guid visitId,
        Func<Visit, Task<T>> mutation,
        string failurePrefix,
        CancellationToken cancellationToken);
}
