namespace TowerOps.Infrastructure.Services;

using TowerOps.Application.Common.Interfaces;

public class DateTimeService : IDateTimeService
{
    public DateTime Now => DateTime.Now;
    public DateTime UtcNow => DateTime.UtcNow;
}