
using Microsoft.Extensions.Logging;
using TelecomPM.Application.Common.Interfaces;

namespace TelecomPM.Infrastructure.Services;

public class NotificationService : INotificationService
{
    private readonly IEmailService _emailService;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        IEmailService emailService,
        ILogger<NotificationService> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    public async Task SendEmailAsync(
        string to,
        string subject,
        string body,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _emailService.SendEmailAsync(to, subject, body, cancellationToken);
            _logger.LogInformation("Email sent to {To} with subject {Subject}", to, subject);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {To}", to);
            throw;
        }
    }

    public async Task SendPushNotificationAsync(
        Guid userId,
        string title,
        string message,
        CancellationToken cancellationToken = default)
    {
        // TODO: Implement push notification (Firebase, SignalR, etc.)
        _logger.LogInformation(
            "Push notification sent to user {UserId}: {Title} - {Message}",
            userId, title, message);

        await Task.CompletedTask;
    }

    public async Task SendSmsAsync(
        string phoneNumber,
        string message,
        CancellationToken cancellationToken = default)
    {
        // TODO: Implement SMS service (Twilio, etc.)
        _logger.LogInformation(
            "SMS sent to {PhoneNumber}: {Message}",
            phoneNumber, message);

        await Task.CompletedTask;
    }
}