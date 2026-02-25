namespace TowerOps.Infrastructure.Persistence.Repositories;

using TowerOps.Domain.Entities.AuditLogs;
using TowerOps.Domain.Interfaces.Repositories;

public class AuditLogRepository : Repository<AuditLog, Guid>, IAuditLogRepository
{
    public AuditLogRepository(ApplicationDbContext context) : base(context)
    {
    }
}
