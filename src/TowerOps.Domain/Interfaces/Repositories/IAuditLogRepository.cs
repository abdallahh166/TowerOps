using TowerOps.Domain.Entities.AuditLogs;

namespace TowerOps.Domain.Interfaces.Repositories;

public interface IAuditLogRepository : IRepository<AuditLog, Guid>
{
}
