using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using TelecomPM.Application.Common.Interfaces;
using TelecomPM.Domain.Entities.SystemSettings;
using TelecomPM.Domain.Interfaces.Repositories;
using TelecomPM.Infrastructure.Services;
using Xunit;

namespace TelecomPM.Infrastructure.Tests.Services;

public class SystemSettingsServiceTests
{
    [Fact]
    public async Task GetAsync_ShouldReturnDefaultValue_WhenKeyNotFound()
    {
        var repository = new Mock<ISystemSettingsRepository>();
        repository
            .Setup(r => r.GetAsync("Missing:Key", It.IsAny<CancellationToken>()))
            .ReturnsAsync((SystemSetting?)null);

        var unitOfWork = new Mock<IUnitOfWork>();
        var encryption = new Mock<ISettingsEncryptionService>();

        var service = new SystemSettingsService(
            repository.Object,
            unitOfWork.Object,
            encryption.Object,
            new MemoryCache(new MemoryCacheOptions()));

        var value = await service.GetAsync("Missing:Key", 123);

        value.Should().Be(123);
    }

    [Fact]
    public async Task SetAsync_ShouldEncryptSecretValuesBeforeStoring()
    {
        var repository = new Mock<ISystemSettingsRepository>();
        repository
            .Setup(r => r.GetAsync("Notifications:Twilio:AuthToken", It.IsAny<CancellationToken>()))
            .ReturnsAsync((SystemSetting?)null);

        var unitOfWork = new Mock<IUnitOfWork>();
        var encryption = new Mock<ISettingsEncryptionService>();
        encryption.Setup(e => e.Encrypt("raw-token")).Returns("encrypted-token");

        SystemSetting? captured = null;
        repository
            .Setup(r => r.UpsertAsync(It.IsAny<SystemSetting>(), It.IsAny<CancellationToken>()))
            .Callback<SystemSetting, CancellationToken>((s, _) => captured = s)
            .Returns(Task.CompletedTask);

        var service = new SystemSettingsService(
            repository.Object,
            unitOfWork.Object,
            encryption.Object,
            new MemoryCache(new MemoryCacheOptions()));

        await service.SetAsync("Notifications:Twilio:AuthToken", "raw-token", "admin");

        captured.Should().NotBeNull();
        captured!.IsEncrypted.Should().BeTrue();
        captured.Value.Should().Be("encrypted-token");
        unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
