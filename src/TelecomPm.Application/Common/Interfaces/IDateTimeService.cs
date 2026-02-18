using System;

namespace TelecomPM.Application.Common.Interfaces;

public interface IDateTimeService
{
    DateTime Now { get; }
    DateTime UtcNow { get; }
}