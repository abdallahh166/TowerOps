using FluentAssertions;
using TelecomPM.Domain.Entities.Assets;
using TelecomPM.Domain.Enums;
using Xunit;

namespace TelecomPM.Domain.Tests.Entities;

public class AssetTests
{
    [Fact]
    public void Create_ShouldInitializeWithActiveStatus()
    {
        var asset = Asset.Create("CAI001", AssetType.Rectifier, "Delta", "D1", "SN1", DateTime.UtcNow, DateTime.UtcNow.AddDays(30));

        asset.Status.Should().Be(AssetStatus.Active);
        asset.AssetCode.Should().StartWith("AST-CAI001-RECTIFIER-");
    }

    [Fact]
    public void RecordService_ShouldAddServiceHistoryEntry()
    {
        var asset = Asset.Create("CAI001", AssetType.Battery, "Brand", "Model", "SN1", DateTime.UtcNow, null);

        asset.RecordService("Inspection", "eng-1", Guid.NewGuid(), "ok");

        asset.ServiceHistory.Should().HaveCount(1);
        asset.LastServicedAtUtc.Should().NotBeNull();
    }

    [Fact]
    public void MarkFaulty_ShouldRaiseAssetFaultedEvent()
    {
        var asset = Asset.Create("CAI001", AssetType.Router, "Brand", "Model", "SN1", DateTime.UtcNow, null);

        asset.MarkFaulty("No power", "eng-1");

        asset.Status.Should().Be(AssetStatus.Faulty);
        asset.DomainEvents.Should().Contain(e => e.GetType().Name == "AssetFaultedEvent");
    }

    [Fact]
    public void Replace_ShouldSetReplacementFields()
    {
        var asset = Asset.Create("CAI001", AssetType.Router, "Brand", "Model", "SN1", DateTime.UtcNow, null);
        var newAssetId = Guid.NewGuid();

        asset.Replace(newAssetId);

        asset.Status.Should().Be(AssetStatus.Replaced);
        asset.ReplacedByAssetId.Should().Be(newAssetId);
        asset.ReplacedAtUtc.Should().NotBeNull();
    }

    [Fact]
    public void IsWarrantyExpired_ShouldReturnExpectedValue()
    {
        var expired = Asset.Create("CAI001", AssetType.Battery, "Brand", "Model", "SN1", DateTime.UtcNow.AddYears(-1), DateTime.UtcNow.AddDays(-1));
        var valid = Asset.Create("CAI002", AssetType.Battery, "Brand", "Model", "SN2", DateTime.UtcNow, DateTime.UtcNow.AddDays(10));

        expired.IsWarrantyExpired().Should().BeTrue();
        valid.IsWarrantyExpired().Should().BeFalse();
    }
}
