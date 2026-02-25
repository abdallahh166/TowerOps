using TowerOps.Domain.Entities.ApprovalRecords;

namespace TowerOps.Domain.Interfaces.Repositories;

public interface IApprovalRecordRepository : IRepository<ApprovalRecord, Guid>
{
}
