namespace TowerOps.Infrastructure.Services;

using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using TowerOps.Application.Common.Interfaces;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ISystemSettingsService _systemSettingsService;
    private readonly ILogger<EmailService> _logger;

    public EmailService(
        IConfiguration configuration,
        ISystemSettingsService systemSettingsService,
        ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _systemSettingsService = systemSettingsService;
        _logger = logger;
    }

    public async Task SendEmailAsync(
        string to,
        string subject,
        string body,
        CancellationToken cancellationToken = default)
    {
        var (smtpServer, smtpPort, senderEmail, senderName, username, password) =
            await ResolveSmtpSettingsAsync(cancellationToken);

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(senderName, senderEmail));
        message.To.Add(new MailboxAddress("", to));
        message.Subject = subject;

        var builder = new BodyBuilder { HtmlBody = body };
        message.Body = builder.ToMessageBody();

        using var client = new SmtpClient();
        try
        {
            await client.ConnectAsync(smtpServer, smtpPort, SecureSocketOptions.StartTls, cancellationToken);
            await client.AuthenticateAsync(username, password, cancellationToken);
            await client.SendAsync(message, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);

            _logger.LogInformation("Email sent successfully to {To}", to);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {To}", to);
            throw;
        }
    }

    public async Task SendBulkEmailAsync(
        IEnumerable<string> recipients,
        string subject,
        string body,
        CancellationToken cancellationToken = default)
    {
        var tasks = recipients.Select(recipient =>
            SendEmailAsync(recipient, subject, body, cancellationToken));

        await Task.WhenAll(tasks);
    }

    public async Task SendEmailWithAttachmentAsync(
        string to,
        string subject,
        string body,
        byte[] attachmentData,
        string attachmentFileName,
        CancellationToken cancellationToken = default)
    {
        var (smtpServer, smtpPort, senderEmail, senderName, username, password) =
            await ResolveSmtpSettingsAsync(cancellationToken);

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(senderName, senderEmail));
        message.To.Add(new MailboxAddress("", to));
        message.Subject = subject;

        var builder = new BodyBuilder { HtmlBody = body };

        // Add attachment
        if (attachmentData != null && attachmentData.Length > 0)
        {
            builder.Attachments.Add(attachmentFileName, attachmentData);
        }

        message.Body = builder.ToMessageBody();

        using var client = new SmtpClient();
        try
        {
            await client.ConnectAsync(smtpServer, smtpPort, SecureSocketOptions.StartTls, cancellationToken);
            await client.AuthenticateAsync(username, password, cancellationToken);
            await client.SendAsync(message, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);

            _logger.LogInformation("Email with attachment sent successfully to {To}", to);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email with attachment to {To}", to);
            throw;
        }
    }

    private async Task<(string smtpServer, int smtpPort, string senderEmail, string senderName, string username, string password)> ResolveSmtpSettingsAsync(CancellationToken cancellationToken)
    {
        var fallbackSection = _configuration.GetSection("EmailSettings");

        var smtpServer = await _systemSettingsService.GetAsync(
            "Notifications:Email:SmtpHost",
            fallbackSection["SmtpServer"] ?? string.Empty,
            cancellationToken);

        var smtpPort = await _systemSettingsService.GetAsync(
            "Notifications:Email:SmtpPort",
            int.TryParse(fallbackSection["SmtpPort"], out var configuredPort) ? configuredPort : 587,
            cancellationToken);

        var senderEmail = await _systemSettingsService.GetAsync(
            "Notifications:Email:FromAddress",
            fallbackSection["SenderEmail"] ?? string.Empty,
            cancellationToken);

        var senderName = await _systemSettingsService.GetAsync(
            "Company:Name",
            fallbackSection["SenderName"] ?? "TowerOps System",
            cancellationToken);

        var username = await _systemSettingsService.GetAsync(
            "Notifications:Email:Username",
            fallbackSection["Username"] ?? string.Empty,
            cancellationToken);

        var password = await _systemSettingsService.GetAsync(
            "Notifications:Email:Password",
            fallbackSection["Password"] ?? string.Empty,
            cancellationToken);

        return (smtpServer, smtpPort, senderEmail, senderName, username, password);
    }
}
