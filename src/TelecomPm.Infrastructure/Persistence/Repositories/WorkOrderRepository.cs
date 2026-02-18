namespace TelecomPM.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using TelecomPM.Domain.Entities.WorkOrders;
using TelecomPM.Domain.Interfaces.Repositories;

public class WorkOrderRepository : Repository<WorkOrder, Guid>, IWorkOrderRepository
{
    public WorkOrderRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<WorkOrder?> GetByWoNumberAsync(string woNumber, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(w => w.WoNumber == woNumber, cancellationToken);
    }
}
