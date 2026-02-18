namespace TelecomPM.Infrastructure.Services;

public sealed class PushNotificationOptions
{
    public string? SignalRWebhookUrl { get; set; }
    public string? FirebaseServerKey { get; set; }
    public string? FirebaseEndpoint { get; set; } = "https://fcm.googleapis.com/fcm/send";
    public string TopicPrefix { get; set; } = "user";
}

public sealed class TwilioOptions
{
    public string? AccountSid { get; set; }
    public string? AuthToken { get; set; }
    public string? FromPhoneNumber { get; set; }
    public string? EndpointBaseUrl { get; set; } = "https://api.twilio.com";
}
