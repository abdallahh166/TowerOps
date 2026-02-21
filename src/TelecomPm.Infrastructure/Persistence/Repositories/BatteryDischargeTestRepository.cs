namespace TelecomPM.Infrastructure.Persistence.Repositories;

using TelecomPM.Domain.Entities.BatteryDischargeTests;
using TelecomPM.Domain.Interfaces.Repositories;

public class BatteryDischargeTestRepository : Repository<BatteryDischargeTest, Guid>, IBatteryDischargeTestRepository
{
    public BatteryDischargeTestRepository(ApplicationDbContext context) : base(context)
    {
    }
}
