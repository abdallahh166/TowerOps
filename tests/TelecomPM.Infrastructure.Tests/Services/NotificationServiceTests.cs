using System.Net;
using System.Net.Http;
using System.Text;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using TelecomPM.Application.Common.Interfaces;
using TelecomPM.Infrastructure.Services;
using Xunit;

namespace TelecomPM.Infrastructure.Tests.Services;

public class NotificationServiceTests
{
    [Fact]
    public async Task SendPushNotificationAsync_WithSignalRAndFirebase_ShouldCallBothEndpoints()
    {
        var handler = new QueueHttpMessageHandler(new[]
        {
            new HttpResponseMessage(HttpStatusCode.OK),
            new HttpResponseMessage(HttpStatusCode.OK)
        });

        var factory = BuildFactory(handler);
        var service = new NotificationService(
            Mock.Of<IEmailService>(),
            factory,
            Options.Create(new PushNotificationOptions
            {
                SignalRWebhookUrl = "https://signalr.example/push",
                FirebaseServerKey = "firebase-key"
            }),
            Options.Create(new TwilioOptions()),
            Mock.Of<ILogger<NotificationService>>());

        await service.SendPushNotificationAsync(Guid.NewGuid(), "Title", "Message");

        handler.Requests.Should().HaveCount(2);
        handler.Requests.Should().Contain(r => r.RequestUri!.Host.Contains("signalr"));
        handler.Requests.Should().Contain(r => r.RequestUri!.Host.Contains("fcm.googleapis.com"));
    }

    [Fact]
    public async Task SendSmsAsync_WithTwilioConfig_ShouldSendRequest()
    {
        var handler = new QueueHttpMessageHandler(new[] { new HttpResponseMessage(HttpStatusCode.Created) });
        var factory = BuildFactory(handler);
        var service = new NotificationService(
            Mock.Of<IEmailService>(),
            factory,
            Options.Create(new PushNotificationOptions()),
            Options.Create(new TwilioOptions
            {
                AccountSid = "AC123",
                AuthToken = "token",
                FromPhoneNumber = "+201000000000"
            }),
            Mock.Of<ILogger<NotificationService>>());

        await service.SendSmsAsync("+201111111111", "Hello");

        handler.Requests.Should().ContainSingle();
        var request = handler.Requests.Single();
        request.Headers.Authorization.Should().NotBeNull();
        request.Headers.Authorization!.Scheme.Should().Be("Basic");
        var body = await request.Content!.ReadAsStringAsync();
        body.Should().Contain("To=%2B201111111111");
    }

    private static IHttpClientFactory BuildFactory(HttpMessageHandler handler)
    {
        var client = new HttpClient(handler);
        var factory = new Mock<IHttpClientFactory>();
        factory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(client);
        return factory.Object;
    }

    private sealed class QueueHttpMessageHandler : HttpMessageHandler
    {
        private readonly Queue<HttpResponseMessage> _responses;

        public QueueHttpMessageHandler(IEnumerable<HttpResponseMessage> responses)
        {
            _responses = new Queue<HttpResponseMessage>(responses);
        }

        public List<HttpRequestMessage> Requests { get; } = new();

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Requests.Add(CloneRequest(request));
            return Task.FromResult(_responses.Count == 0
                ? new HttpResponseMessage(HttpStatusCode.OK)
                : _responses.Dequeue());
        }

        private static HttpRequestMessage CloneRequest(HttpRequestMessage source)
        {
            var clone = new HttpRequestMessage(source.Method, source.RequestUri);
            foreach (var header in source.Headers)
            {
                clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            if (source.Content != null)
            {
                var content = source.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                clone.Content = new StringContent(content, Encoding.UTF8, source.Content.Headers.ContentType?.MediaType);
            }

            return clone;
        }
    }
}
