
using System;
using TelecomPM.Domain.Enums;

namespace TelecomPM.Application.Common.Interfaces;

public interface ICurrentUserService
{
    Guid UserId { get; }
    string Email { get; }
    UserRole Role { get; }
    Guid OfficeId { get; }
    bool IsAuthenticated { get; }
}