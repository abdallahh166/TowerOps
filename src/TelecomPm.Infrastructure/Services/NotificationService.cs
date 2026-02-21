using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TelecomPM.Application.Common.Interfaces;

namespace TelecomPM.Infrastructure.Services;

public class NotificationService : INotificationService
{
    private readonly IEmailService _emailService;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly PushNotificationOptions _pushOptions;
    private readonly TwilioOptions _twilioOptions;
    private readonly ISystemSettingsService _systemSettingsService;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        IEmailService emailService,
        IHttpClientFactory httpClientFactory,
        IOptions<PushNotificationOptions> pushOptions,
        IOptions<TwilioOptions> twilioOptions,
        ISystemSettingsService systemSettingsService,
        ILogger<NotificationService> logger)
    {
        _emailService = emailService;
        _httpClientFactory = httpClientFactory;
        _pushOptions = pushOptions.Value;
        _twilioOptions = twilioOptions.Value;
        _systemSettingsService = systemSettingsService;
        _logger = logger;
    }

    public async Task SendEmailAsync(
        string to,
        string subject,
        string body,
        CancellationToken cancellationToken = default)
    {
        await _emailService.SendEmailAsync(to, subject, body, cancellationToken);
        _logger.LogInformation("Email sent to {To} with subject {Subject}", to, subject);
    }

    public async Task SendPushNotificationAsync(
        Guid userId,
        string title,
        string message,
        CancellationToken cancellationToken = default)
    {
        var signalRWebhookUrl = await _systemSettingsService.GetAsync(
            "Notifications:Push:SignalRWebhookUrl",
            _pushOptions.SignalRWebhookUrl ?? string.Empty,
            cancellationToken);

        var firebaseServerKey = await _systemSettingsService.GetAsync(
            "Notifications:Firebase:ServerKey",
            _pushOptions.FirebaseServerKey ?? string.Empty,
            cancellationToken);

        var firebaseEndpoint = await _systemSettingsService.GetAsync(
            "Notifications:Firebase:Endpoint",
            _pushOptions.FirebaseEndpoint ?? "https://fcm.googleapis.com/fcm/send",
            cancellationToken);

        var topicPrefix = await _systemSettingsService.GetAsync(
            "Notifications:Firebase:TopicPrefix",
            _pushOptions.TopicPrefix,
            cancellationToken);

        var hasSignalR = !string.IsNullOrWhiteSpace(signalRWebhookUrl);
        var hasFirebase = !string.IsNullOrWhiteSpace(firebaseServerKey);

        if (!hasSignalR && !hasFirebase)
        {
            _logger.LogWarning("Push notification skipped for user {UserId}. SignalR/Firebase are not configured.", userId);
            return;
        }

        if (hasSignalR)
        {
            await SendSignalRNotificationAsync(userId, title, message, signalRWebhookUrl, cancellationToken);
        }

        if (hasFirebase)
        {
            await SendFirebaseNotificationAsync(userId, title, message, firebaseServerKey, firebaseEndpoint, topicPrefix, cancellationToken);
        }
    }

    public async Task SendSmsAsync(
        string phoneNumber,
        string message,
        CancellationToken cancellationToken = default)
    {
        var accountSid = await _systemSettingsService.GetAsync(
            "Notifications:Twilio:AccountSid",
            _twilioOptions.AccountSid ?? string.Empty,
            cancellationToken);

        var authToken = await _systemSettingsService.GetAsync(
            "Notifications:Twilio:AuthToken",
            _twilioOptions.AuthToken ?? string.Empty,
            cancellationToken);

        var fromNumber = await _systemSettingsService.GetAsync(
            "Notifications:Twilio:FromNumber",
            _twilioOptions.FromPhoneNumber ?? string.Empty,
            cancellationToken);

        if (string.IsNullOrWhiteSpace(accountSid) ||
            string.IsNullOrWhiteSpace(authToken) ||
            string.IsNullOrWhiteSpace(fromNumber))
        {
            _logger.LogWarning("SMS skipped for {PhoneNumber}. Twilio is not configured.", phoneNumber);
            return;
        }

        var endpointBase = _twilioOptions.EndpointBaseUrl?.TrimEnd('/') ?? "https://api.twilio.com";
        var url = $"{endpointBase}/2010-04-01/Accounts/{accountSid}/Messages.json";

        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["To"] = phoneNumber,
            ["From"] = fromNumber,
            ["Body"] = message
        });

        var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = content
        };

        var authBytes = Encoding.ASCII.GetBytes($"{accountSid}:{authToken}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authBytes));

        var client = _httpClientFactory.CreateClient(nameof(NotificationService));
        var response = await client.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException($"Twilio SMS failed with status {(int)response.StatusCode}: {body}");
        }

        _logger.LogInformation("SMS sent to {PhoneNumber}", phoneNumber);
    }

    private async Task SendSignalRNotificationAsync(
        Guid userId,
        string title,
        string message,
        string signalRWebhookUrl,
        CancellationToken cancellationToken)
    {
        var payload = new
        {
            userId,
            title,
            message,
            sentAtUtc = DateTime.UtcNow
        };

        var request = new HttpRequestMessage(HttpMethod.Post, signalRWebhookUrl)
        {
            Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
        };

        var client = _httpClientFactory.CreateClient(nameof(NotificationService));
        var response = await client.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException($"SignalR push failed with status {(int)response.StatusCode}: {body}");
        }

        _logger.LogInformation("SignalR push sent to user {UserId}", userId);
    }

    private async Task SendFirebaseNotificationAsync(
        Guid userId,
        string title,
        string message,
        string firebaseServerKey,
        string firebaseEndpoint,
        string topicPrefix,
        CancellationToken cancellationToken)
    {
        var endpoint = string.IsNullOrWhiteSpace(firebaseEndpoint)
            ? "https://fcm.googleapis.com/fcm/send"
            : firebaseEndpoint;

        topicPrefix = string.IsNullOrWhiteSpace(topicPrefix) ? "user" : topicPrefix;
        var topic = $"/topics/{topicPrefix}-{userId:N}";

        var payload = new
        {
            to = topic,
            notification = new
            {
                title,
                body = message
            },
            data = new
            {
                userId,
                title,
                message,
                sentAtUtc = DateTime.UtcNow
            }
        };

        var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
        {
            Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
        };

        request.Headers.TryAddWithoutValidation("Authorization", $"key={firebaseServerKey}");

        var client = _httpClientFactory.CreateClient(nameof(NotificationService));
        var response = await client.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException($"Firebase push failed with status {(int)response.StatusCode}: {body}");
        }

        _logger.LogInformation("Firebase push sent to user topic {Topic}", topic);
    }
}
