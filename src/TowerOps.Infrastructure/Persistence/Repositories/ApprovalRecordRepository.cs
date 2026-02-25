namespace TowerOps.Infrastructure.Persistence.Repositories;

using TowerOps.Domain.Entities.ApprovalRecords;
using TowerOps.Domain.Interfaces.Repositories;

public class ApprovalRecordRepository : Repository<ApprovalRecord, Guid>, IApprovalRecordRepository
{
    public ApprovalRecordRepository(ApplicationDbContext context) : base(context)
    {
    }
}
