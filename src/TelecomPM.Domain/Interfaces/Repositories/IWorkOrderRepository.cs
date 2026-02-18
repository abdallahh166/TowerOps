namespace TelecomPM.Domain.Interfaces.Repositories;

using TelecomPM.Domain.Entities.WorkOrders;

public interface IWorkOrderRepository : IRepository<WorkOrder, Guid>
{
    Task<WorkOrder?> GetByWoNumberAsync(string woNumber, CancellationToken cancellationToken = default);
}
