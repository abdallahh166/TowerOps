namespace TowerOps.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TowerOps.Api.Contracts.Settings;
using TowerOps.Api.Authorization;
using TowerOps.Api.Mappings;
using TowerOps.Application.Commands.Settings.UpsertSystemSettings;
using TowerOps.Application.Common.Interfaces;
using TowerOps.Application.DTOs.Settings;
using TowerOps.Application.Queries.Settings.GetAllSystemSettings;
using TowerOps.Application.Queries.Settings.GetSystemSettingsByGroup;
using TowerOps.Domain.Interfaces.Repositories;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = ApiAuthorizationPolicies.CanManageSettings)]
public sealed class SettingsController : ApiControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly IEmailService _emailService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUserRepository _userRepository;

    public SettingsController(
        INotificationService notificationService,
        IEmailService emailService,
        ICurrentUserService currentUserService,
        IUserRepository userRepository)
    {
        _notificationService = notificationService;
        _emailService = emailService;
        _currentUserService = currentUserService;
        _userRepository = userRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery(Name = "page")] int page = 1,
        [FromQuery] int pageSize = 25,
        [FromQuery] string? sortBy = null,
        [FromQuery] string sortDir = "desc",
        CancellationToken cancellationToken = default)
    {
        var safePage = page < 1 ? 1 : page;
        var safePageSize = Math.Clamp(pageSize, 1, 100);
        if (!TryResolveSort(
                sortBy,
                sortDir,
                new[] { "group", "key", "dataType", "updatedAtUtc" },
                defaultSortBy: "group",
                out var resolvedSortBy,
                out var sortDescending,
                out var sortError))
        {
            return sortError!;
        }

        var result = await Mediator.Send(
            new GetAllSystemSettingsQuery
            {
                Page = safePage,
                PageSize = safePageSize,
                SortBy = resolvedSortBy,
                SortDescending = sortDescending
            },
            cancellationToken);
        if (!result.IsSuccess || result.Value is null)
            return HandleResult(result);

        var mapped = result.Value.Items.Select(ToResponse).ToList();
        return Ok(mapped.ToPagedResponse(
            result.Value.PageNumber,
            result.Value.PageSize,
            result.Value.TotalCount));
    }

    [HttpGet("{group}")]
    public async Task<IActionResult> GetByGroup(string group, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(
            new GetSystemSettingsByGroupQuery { Group = group },
            cancellationToken);

        if (!result.IsSuccess || result.Value is null)
            return HandleResult(result);

        var response = result.Value
            .OrderBy(s => s.Key)
            .Select(ToResponse)
            .ToList();

        return Ok(response);
    }

    [HttpPut]
    public async Task<IActionResult> Upsert(
        [FromBody] IReadOnlyList<UpsertSystemSettingRequest> request,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(
            new UpsertSystemSettingsCommand
            {
                UpdatedBy = ResolveUpdatedBy(),
                Settings = request.Select(s => new UpsertSystemSettingItem
                {
                    Key = s.Key,
                    Value = s.Value,
                    Group = s.Group,
                    DataType = s.DataType,
                    Description = s.Description,
                    IsEncrypted = s.IsEncrypted
                }).ToList()
            },
            cancellationToken);

        return HandleResult(result);
    }

    [HttpPost("test/{service}")]
    public async Task<IActionResult> TestConnection(string service, CancellationToken cancellationToken)
    {
        var normalized = service.Trim().ToLowerInvariant();
        return normalized switch
        {
            "twilio" => await TestTwilioAsync(cancellationToken),
            "email" => await TestEmailAsync(cancellationToken),
            "firebase" => await TestFirebaseAsync(cancellationToken),
            _ => Failure(LocalizedText.Get("SettingsUnsupportedService", "Unsupported service. Use twilio, email, or firebase."))
        };
    }

    private async Task<IActionResult> TestTwilioAsync(CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAuthenticated || _currentUserService.UserId == Guid.Empty)
        {
            return UnauthorizedFailure();
        }

        var user = await _userRepository.GetByIdAsNoTrackingAsync(_currentUserService.UserId, cancellationToken);
        if (user is null || string.IsNullOrWhiteSpace(user.PhoneNumber))
        {
            return Failure("Current user phone number is not available.");
        }

        await _notificationService.SendSmsAsync(user.PhoneNumber, "TowerOps Twilio test message.", cancellationToken);
        return Ok(new { message = "Twilio test sent." });
    }

    private async Task<IActionResult> TestEmailAsync(CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAuthenticated || string.IsNullOrWhiteSpace(_currentUserService.Email))
        {
            return UnauthorizedFailure();
        }

        await _emailService.SendEmailAsync(
            _currentUserService.Email,
            "TowerOps Email Test",
            "<p>This is a TowerOps SMTP test email.</p>",
            cancellationToken);

        return Ok(new { message = "Email test sent." });
    }

    private async Task<IActionResult> TestFirebaseAsync(CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAuthenticated || _currentUserService.UserId == Guid.Empty)
        {
            return UnauthorizedFailure();
        }

        await _notificationService.SendPushNotificationAsync(
            _currentUserService.UserId,
            "TowerOps Push Test",
            "This is a Firebase push notification test.",
            cancellationToken);

        return Ok(new { message = "Firebase test sent." });
    }

    private static SystemSettingResponse ToResponse(SystemSettingDto setting)
    {
        return new SystemSettingResponse
        {
            Key = setting.Key,
            Group = setting.Group,
            DataType = setting.DataType,
            Description = setting.Description,
            IsEncrypted = setting.IsEncrypted,
            UpdatedAtUtc = setting.UpdatedAtUtc,
            UpdatedBy = setting.UpdatedBy,
            Value = setting.Value
        };
    }

    private string ResolveUpdatedBy()
    {
        if (_currentUserService.IsAuthenticated && !string.IsNullOrWhiteSpace(_currentUserService.Email))
            return _currentUserService.Email;

        return "System";
    }
}
