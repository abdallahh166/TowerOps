namespace TelecomPM.Infrastructure.Services;

using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using TelecomPM.Application.Common.Interfaces;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendEmailAsync(
        string to,
        string subject,
        string body,
        CancellationToken cancellationToken = default)
    {
        var emailSettings = _configuration.GetSection("EmailSettings");
        var smtpServer = emailSettings["SmtpServer"];
        var smtpPort = int.Parse(emailSettings["SmtpPort"] ?? "587");
        var senderEmail = emailSettings["SenderEmail"];
        var senderName = emailSettings["SenderName"];
        var username = emailSettings["Username"];
        var password = emailSettings["Password"];

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
        var emailSettings = _configuration.GetSection("EmailSettings");
        var smtpServer = emailSettings["SmtpServer"];
        var smtpPort = int.Parse(emailSettings["SmtpPort"] ?? "587");
        var senderEmail = emailSettings["SenderEmail"];
        var senderName = emailSettings["SenderName"];
        var username = emailSettings["Username"];
        var password = emailSettings["Password"];

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
}