namespace TelecomPM.Infrastructure.Services;

using TelecomPM.Application.Common.Interfaces;

public class DateTimeService : IDateTimeService
{
    public DateTime Now => DateTime.Now;
    public DateTime UtcNow => DateTime.UtcNow;
}