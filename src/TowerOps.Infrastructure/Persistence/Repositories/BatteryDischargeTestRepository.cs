namespace TowerOps.Infrastructure.Persistence.Repositories;

using TowerOps.Domain.Entities.BatteryDischargeTests;
using TowerOps.Domain.Interfaces.Repositories;

public class BatteryDischargeTestRepository : Repository<BatteryDischargeTest, Guid>, IBatteryDischargeTestRepository
{
    public BatteryDischargeTestRepository(ApplicationDbContext context) : base(context)
    {
    }
}
