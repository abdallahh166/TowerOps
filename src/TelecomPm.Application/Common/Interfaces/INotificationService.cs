using System;
using System.Threading;
using System.Threading.Tasks;

namespace TelecomPM.Application.Common.Interfaces;

public interface INotificationService
{
    Task SendEmailAsync(
        string to,
        string subject,
        string body,
        CancellationToken cancellationToken = default);

    Task SendPushNotificationAsync(
        Guid userId,
        string title,
        string message,
        CancellationToken cancellationToken = default);

    Task SendSmsAsync(
        string phoneNumber,
        string message,
        CancellationToken cancellationToken = default);
}