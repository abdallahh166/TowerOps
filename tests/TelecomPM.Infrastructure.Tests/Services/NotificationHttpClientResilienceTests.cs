using System.Net;
using System.Net.Http;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using TelecomPM.Application.Common.Interfaces;
using TelecomPM.Infrastructure;
using TelecomPM.Infrastructure.Services;
using Xunit;

namespace TelecomPM.Infrastructure.Tests.Services;

public class NotificationHttpClientResilienceTests
{
    [Fact]
    public async Task SendSmsAsync_ShouldRetryThreeTimes_OnTransientServerFailures()
    {
        var sequenceHandler = new SequenceHandler(
            new HttpResponseMessage(HttpStatusCode.ServiceUnavailable),
            new HttpResponseMessage(HttpStatusCode.ServiceUnavailable),
            new HttpResponseMessage(HttpStatusCode.ServiceUnavailable),
            new HttpResponseMessage(HttpStatusCode.Created));

        var services = new ServiceCollection();
        services.AddLogging();

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = "Server=(localdb)\\MSSQLLocalDB;Database=TelecomPmTest;Trusted_Connection=True;TrustServerCertificate=True"
            })
            .Build();

        services.AddInfrastructure(configuration);

        // Override primary handler for the named notification client while keeping resilience pipeline.
        services.AddHttpClient(nameof(NotificationService))
            .ConfigurePrimaryHttpMessageHandler(() => sequenceHandler);

        var provider = services.BuildServiceProvider();
        var factory = provider.GetRequiredService<IHttpClientFactory>();

        var sut = new NotificationService(
            Mock.Of<IEmailService>(),
            factory,
            Options.Create(new PushNotificationOptions()),
            Options.Create(new TwilioOptions
            {
                AccountSid = "AC123",
                AuthToken = "token",
                FromPhoneNumber = "+201000000000",
                EndpointBaseUrl = "https://twilio.test"
            }),
            Mock.Of<ILogger<NotificationService>>());

        await sut.SendSmsAsync("+201111111111", "hello");

        sequenceHandler.Attempts.Should().Be(4);
    }

    private sealed class SequenceHandler : HttpMessageHandler
    {
        private readonly Queue<HttpResponseMessage> _responses;

        public SequenceHandler(params HttpResponseMessage[] responses)
        {
            _responses = new Queue<HttpResponseMessage>(responses);
        }

        public int Attempts { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Attempts++;
            return Task.FromResult(_responses.Count > 0
                ? _responses.Dequeue()
                : new HttpResponseMessage(HttpStatusCode.OK));
        }
    }
}
