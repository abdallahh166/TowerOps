namespace TelecomPm.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TelecomPm.Api.Contracts.Settings;
using TelecomPM.Api.Authorization;
using TelecomPM.Application.Common.Interfaces;
using TelecomPM.Domain.Entities.SystemSettings;
using TelecomPM.Domain.Interfaces.Repositories;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = ApiAuthorizationPolicies.CanManageSettings)]
public sealed class SettingsController : ApiControllerBase
{
    private readonly ISystemSettingsRepository _settingsRepository;
    private readonly ISettingsEncryptionService _settingsEncryptionService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;
    private readonly IEmailService _emailService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUserRepository _userRepository;

    public SettingsController(
        ISystemSettingsRepository settingsRepository,
        ISettingsEncryptionService settingsEncryptionService,
        IUnitOfWork unitOfWork,
        INotificationService notificationService,
        IEmailService emailService,
        ICurrentUserService currentUserService,
        IUserRepository userRepository)
    {
        _settingsRepository = settingsRepository;
        _settingsEncryptionService = settingsEncryptionService;
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
        _emailService = emailService;
        _currentUserService = currentUserService;
        _userRepository = userRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var settings = await _settingsRepository.GetAllAsNoTrackingAsync(cancellationToken);

        var grouped = settings
            .GroupBy(s => s.Group)
            .OrderBy(g => g.Key)
            .ToDictionary(
                g => g.Key,
                g => g
                    .OrderBy(s => s.Key)
                    .Select(ToResponse)
                    .ToList());

        return Ok(grouped);
    }

    [HttpGet("{group}")]
    public async Task<IActionResult> GetByGroup(string group, CancellationToken cancellationToken)
    {
        var settings = await _settingsRepository.GetByGroupAsync(group, cancellationToken);
        var response = settings
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
        if (request.Count == 0)
        {
            return BadRequest("At least one setting is required.");
        }

        var updatedBy = ResolveUpdatedBy();

        var settings = request
            .Where(s => !string.IsNullOrWhiteSpace(s.Key))
            .Select(s => SystemSetting.Create(
                s.Key.Trim(),
                s.IsEncrypted
                    ? _settingsEncryptionService.Encrypt(s.Value ?? string.Empty)
                    : s.Value ?? string.Empty,
                string.IsNullOrWhiteSpace(s.Group) ? ResolveGroupFromKey(s.Key) : s.Group.Trim(),
                string.IsNullOrWhiteSpace(s.DataType) ? "string" : s.DataType.Trim(),
                s.Description,
                s.IsEncrypted,
                updatedBy))
            .ToList();

        await _settingsRepository.UpsertManyAsync(settings, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Ok();
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
            _ => BadRequest("Unsupported service. Use twilio, email, or firebase.")
        };
    }

    private async Task<IActionResult> TestTwilioAsync(CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAuthenticated || _currentUserService.UserId == Guid.Empty)
        {
            return Unauthorized();
        }

        var user = await _userRepository.GetByIdAsNoTrackingAsync(_currentUserService.UserId, cancellationToken);
        if (user is null || string.IsNullOrWhiteSpace(user.PhoneNumber))
        {
            return BadRequest("Current user phone number is not available.");
        }

        await _notificationService.SendSmsAsync(user.PhoneNumber, "TelecomPM Twilio test message.", cancellationToken);
        return Ok(new { message = "Twilio test sent." });
    }

    private async Task<IActionResult> TestEmailAsync(CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAuthenticated || string.IsNullOrWhiteSpace(_currentUserService.Email))
        {
            return Unauthorized();
        }

        await _emailService.SendEmailAsync(
            _currentUserService.Email,
            "TelecomPM Email Test",
            "<p>This is a TelecomPM SMTP test email.</p>",
            cancellationToken);

        return Ok(new { message = "Email test sent." });
    }

    private async Task<IActionResult> TestFirebaseAsync(CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAuthenticated || _currentUserService.UserId == Guid.Empty)
        {
            return Unauthorized();
        }

        await _notificationService.SendPushNotificationAsync(
            _currentUserService.UserId,
            "TelecomPM Push Test",
            "This is a Firebase push notification test.",
            cancellationToken);

        return Ok(new { message = "Firebase test sent." });
    }

    private static SystemSettingResponse ToResponse(SystemSetting setting)
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
            Value = setting.IsEncrypted ? "***" : setting.Value
        };
    }

    private string ResolveUpdatedBy()
    {
        if (_currentUserService.IsAuthenticated && !string.IsNullOrWhiteSpace(_currentUserService.Email))
            return _currentUserService.Email;

        return "System";
    }

    private static string ResolveGroupFromKey(string key)
    {
        var parts = key.Split(':', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length > 0 ? parts[0] : "General";
    }
}
