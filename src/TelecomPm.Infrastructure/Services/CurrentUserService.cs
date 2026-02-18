namespace TelecomPM.Infrastructure.Services;

using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using TelecomPM.Application.Common.Interfaces;
using TelecomPM.Domain.Enums;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid UserId
    {
        get
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?
                .FindFirstValue(ClaimTypes.NameIdentifier);

            return string.IsNullOrEmpty(userIdClaim) ? Guid.Empty : Guid.Parse(userIdClaim);
        }
    }

    public string Email
    {
        get
        {
            return _httpContextAccessor.HttpContext?.User?
                .FindFirstValue(ClaimTypes.Email) ?? string.Empty;
        }
    }

    public UserRole Role
    {
        get
        {
            var roleClaim = _httpContextAccessor.HttpContext?.User?
                .FindFirstValue(ClaimTypes.Role);

            return string.IsNullOrEmpty(roleClaim)
                ? UserRole.PMEngineer
                : Enum.Parse<UserRole>(roleClaim);
        }
    }

    public Guid OfficeId
    {
        get
        {
            var officeIdClaim = _httpContextAccessor.HttpContext?.User?
                .FindFirstValue("OfficeId");

            return string.IsNullOrEmpty(officeIdClaim) ? Guid.Empty : Guid.Parse(officeIdClaim);
        }
    }

    public bool IsAuthenticated
    {
        get
        {
            return _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
        }
    }
}